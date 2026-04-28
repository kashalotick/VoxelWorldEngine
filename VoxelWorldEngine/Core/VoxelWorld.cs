using System.Numerics;
using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

public class VoxelWorld : IWorldRegion
{
    public IChunkDirtySink? ChunkDirtySink { get; set; }


    public VoxelWorld(int seed)
    {
        Seed = seed;
    }

    public int Seed { get; }
    private readonly object _chunksLock = new();
    private Dictionary<Vector3Int, Chunk> _chunks = new();
    public IReadOnlyDictionary<Vector3Int, Chunk> Chunks => _chunks;

    public void AddChunk(Chunk chunk)
    {
        lock (_chunksLock)
        {
            _chunks[chunk.Position] = chunk;
        }
    }

    public void UpdateChunk(Chunk chunk)
    {
        lock (_chunksLock)
        {
            _chunks[chunk.Position] = chunk;
        }
    }

    public void RemoveChunk(Vector3Int chunkPosition)
    {
        lock (_chunksLock)
        {
            _chunks.Remove(chunkPosition);
        }
    }

    public bool TryGetChunk(Vector3Int chunkPosition, out Chunk chunk)
    {
        lock (_chunksLock)
        {
            return _chunks.TryGetValue(chunkPosition, out chunk);
        }
    }

    public Vector3Int[] GetChunkPositionsSnapshot()
    {
        lock (_chunksLock)
        {
            var result = new Vector3Int[_chunks.Count];
            _chunks.Keys.CopyTo(result, 0);
            return result;
        }
    }

    public bool PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId)
    {
        var chunkPos = Chunk.GlobalToChunk(voxelPositionIndex);
        if (!TryGetChunk(chunkPos, out var chunk))
        {
            return false;
        }

        var result = chunk.PlaceBlock(voxelPositionIndex, blockId);
        if (result)
        {
            chunk.MarkDirty();
            ChunkDirtySink?.MarkDirty(chunkPos);
        }

        return result;
    }

    public void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        Voxel[] data,
        Func<Voxel, Voxel, bool>? canReplace = null
    )
    {
        var firstChunk = Chunk.GlobalToChunk(insertPosition);
        var lastChunk = Chunk.GlobalToChunk(insertPosition + areaSize - Vector3Int.One);

        for (int x = firstChunk.X; x <= lastChunk.X; x++)
        for (int y = firstChunk.Y; y <= lastChunk.Y; y++)
        for (int z = firstChunk.Z; z <= lastChunk.Z; z++)
        {
            var chunkPos = new Vector3Int(x, y, z);
            if (!TryGetChunk(chunkPos, out var chunk))
            {
                continue;
            }

            chunk.ModifyArea(insertPosition, areaSize, data, canReplace);
            chunk.MarkDirty();
            ChunkDirtySink?.MarkDirty(chunkPos);
        }
    }

    public bool TryGetVoxel(Vector3Int voxelPosition, out Voxel voxel)
    {
        var chunkPos = Chunk.GlobalToChunk(voxelPosition);
        if (!TryGetChunk(chunkPos, out var chunk))
        {
            voxel = Voxel.Void;
            return false;
        }

        var localVoxelIndex = Chunk.GlobalToLocal(voxelPosition);
        voxel = chunk.Octree.GetData(localVoxelIndex);
        return true;
    }

    public bool IsSolid(Vector3Int voxelPosition)
    {
        return TryGetVoxel(voxelPosition, out var voxel)
            ? !voxel.IsAir && !voxel.IsVoid
            : true;
    }

    public RayHit Raycast(Ray ray)
    {
        var enumerator = new RayGridEnumerator(ray, Chunk.ChunkSize);

        do
        {
            if (TryGetChunk(enumerator.CurrentPos, out var chunk))
            {
                var hit = chunk.Raycast(ray);
                if (!hit.Voxel.IsAir) return hit;
            }
        } while (enumerator.MoveNext());

        return RayHit.NoHit;
    }
}