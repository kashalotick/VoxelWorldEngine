using System.Numerics;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

public class VoxelWorld : IWorldRegion
{
    public event Action<Chunk> ChunkAdded;
    public event Action<Chunk> ChunkUpdated;
    public event Action<Chunk> ChunkRemoved;


    public VoxelWorld(int seed)
    {
        Seed = seed;
    }

    public int Seed { get; }
    private Dictionary<Vector3Int, Chunk> _chunks = new();
    public IReadOnlyDictionary<Vector3Int, Chunk> Chunks => _chunks;

    private readonly HashSet<Vector3Int> _meshDirtyChunks = new();
    public IReadOnlySet<Vector3Int> MeshDirtyChunks => _meshDirtyChunks;


    public void AddChunk(Chunk chunk)
    {
        _chunks[chunk.Position] = chunk;
        ChunkAdded?.Invoke(chunk);
    }

    public void UpdateChunk(Chunk chunk)
    {
        _chunks[chunk.Position] = chunk;
        ChunkUpdated?.Invoke(chunk);
    }

    public void RemoveChunk(Vector3Int chunkPosition)
    {
        if (_chunks.TryGetValue(chunkPosition, out var chunk))
        {
            ChunkRemoved?.Invoke(chunk); // ???????? ????? ? chunk ?? ? ????????
            _chunks.Remove(chunkPosition);
        }
    }

    public void ClearMeshDirty()
    {
        _meshDirtyChunks.Clear();
    }

    public void MarkChunkMeshClean(Vector3Int chunkPos)
    {
        _meshDirtyChunks.Remove(chunkPos);
    }

    public bool PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId)
    {
        var chunkPos = Chunk.GlobalToChunk(voxelPositionIndex);
        var chunk = _chunks[chunkPos];

        var result = chunk.PlaceBlock(voxelPositionIndex, blockId);
        if (result)
        {
            _meshDirtyChunks.Add(chunkPos);
        }

        return result;
    }

    public void ModifyArea(Vector3Int insertPosition, Vector3Int areaSize, Voxel[] data, Func<Voxel, Voxel, bool>? canReplace = null)
    {
        var firstChunk = Chunk.GlobalToChunk(insertPosition);
        var lastChunk = Chunk.GlobalToChunk(insertPosition + areaSize - Vector3Int.One);
        
        
        for (int x = firstChunk.X; x <= lastChunk.X; x++)
        for (int y = firstChunk.Y; y <= lastChunk.Y; y++)
        for (int z = firstChunk.Z; z <= lastChunk.Z; z++)
        {
            var chunkPos = new Vector3Int(x, y, z);
            var chunk = _chunks[chunkPos];
            chunk.ModifyArea(insertPosition, areaSize, data, canReplace);
            _meshDirtyChunks.Add(chunkPos);
        }
    }
    

    public RayHit Raycast(Ray ray)
    {
        var enumerator = new RayGridEnumerator(ray, Chunk.ChunkSize);

        if (_chunks.TryGetValue(enumerator.CurrentPos, out var firstChunk))
        {
            var hit = firstChunk.Raycast(ray);
            if (!hit.Voxel.IsAir) return hit;
        }

        while (enumerator.MoveNext())
        {
            if (_chunks.TryGetValue(enumerator.CurrentPos, out var chunk))
            {
                var hit = chunk.Raycast(ray);
                if (!hit.Voxel.IsAir) return hit;
            }
        }

        return new RayHit { Voxel = Voxel.Air };
    }}