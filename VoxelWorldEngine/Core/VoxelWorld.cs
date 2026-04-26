using System.Numerics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

// TODO: mb make composite with IWorldStructure for world, chunk, voxel octree
public class VoxelWorld : IRaycastable
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

    public void PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId)
    {
        var chunkPos = Chunk.GlobalToChunk(voxelPositionIndex);
        var localVoxelIndex = Chunk.GlobalToLocal(voxelPositionIndex);

        var chunk = _chunks[chunkPos];


        var isDataChanged = chunk.Octree.SetData(localVoxelIndex, new Voxel(blockId));
        if (isDataChanged)
        {
            chunk.MarkDirty();
            _meshDirtyChunks.Add(chunkPos);
        }
    }

    public void ModifyArea(Vector3Int insertPosition, Vector3Int areaSize, Voxel[] data, Func<Voxel, Voxel, bool>? canReplace = null)
    {
        var firstChunk = Chunk.GlobalToChunk(insertPosition);
        var lastChunk = Chunk.GlobalToChunk(insertPosition + areaSize - Vector3Int.One);
        
        
        for (int x = firstChunk.X; x <= lastChunk.X; x++)
        for (int y = firstChunk.Y; y <= lastChunk.Y; y++)
        for (int z = firstChunk.Z; z <= lastChunk.Z; z++)
            ModifyChunk(new Vector3Int(x, y, z), insertPosition, areaSize, data, canReplace);
    }

    private void ModifyChunk(Vector3Int chunkPosition, Vector3Int insertPosition, Vector3Int areaSize, Voxel[] data, Func<Voxel, Voxel, bool>? canReplace)
    {
        Console.WriteLine($"[VoxelWorld] Executing ModifyRegion at chunk {chunkPosition}, insert position {insertPosition}, area size: {areaSize}");

        var chunk = _chunks[chunkPosition];
        var chunkOriginGlobal = Chunk.ChunkToGlobal(chunkPosition);
        var relativeInsertPosition = insertPosition - chunkOriginGlobal;
        
        chunk.Octree.ModifyArea(relativeInsertPosition, areaSize, data, canReplace);
        
        chunk.MarkDirty();
        _meshDirtyChunks.Add(chunkPosition);
    }
        
    

    public RayHit Raycast(Ray ray)
    {
        var chunkPos = Chunk.GlobalToChunk(ray.Origin.ToVector3Int());
        var dir = ray.Direction;
        var chunkSize = Chunk.ChunkSize;

        // ???? ?? ?????? ???
        var step = new Vector3Int(
            dir.X >= 0 ? 1 : -1,
            dir.Y >= 0 ? 1 : -1,
            dir.Z >= 0 ? 1 : -1
        );

        // ??????? t ????? ?????? ??? ????????? ???? ???? ?? ?????? ???
        var tDelta = new Vector3(
            MathF.Abs(chunkSize / dir.X),
            MathF.Abs(chunkSize / dir.Y),
            MathF.Abs(chunkSize / dir.Z)
        );

        // t ?? ?????? ???? ?? ?????? ???
        var chunkOrigin = chunkPos * chunkSize;
        var tMax = new Vector3(
            dir.X >= 0 ? (chunkOrigin.X + chunkSize - ray.Origin.X) / dir.X : (chunkOrigin.X - ray.Origin.X) / dir.X,
            dir.Y >= 0 ? (chunkOrigin.Y + chunkSize - ray.Origin.Y) / dir.Y : (chunkOrigin.Y - ray.Origin.Y) / dir.Y,
            dir.Z >= 0 ? (chunkOrigin.Z + chunkSize - ray.Origin.Z) / dir.Z : (chunkOrigin.Z - ray.Origin.Z) / dir.Z
        );

        float t = 0;
        while (t <= ray.Length)
        {
            if (_chunks.TryGetValue(chunkPos, out var chunk))
            {
                var localRay = ray with { Origin = ray.Origin - (Vector3)chunk.GlobalPosition };
                // var localRay = ray;

                var hit = chunk.Octree.Raycast(localRay);
                if (!hit.Voxel.IsAir)
                {
                    hit.HitIn += (Vector3)chunk.GlobalPosition;
                    hit.HitOut += (Vector3)chunk.GlobalPosition;
                    return hit;
                }
            }

            // ?????????? ? ????????? ???? ?? ?????????? ????
            if (tMax.X < tMax.Y && tMax.X < tMax.Z)
            {
                chunkPos.X += step.X;
                t = tMax.X;
                tMax.X += tDelta.X;
            }
            else if (tMax.Y < tMax.Z)
            {
                chunkPos.Y += step.Y;
                t = tMax.Y;
                tMax.Y += tDelta.Y;
            }
            else
            {
                chunkPos.Z += step.Z;
                t = tMax.Z;
                tMax.Z += tDelta.Z;
            }
        }

        return new RayHit { Voxel = Voxel.Air };
    }
}