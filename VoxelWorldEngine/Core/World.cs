using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Voxels;
using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core;

public class World
{
    public Seed Seed;
    public Dictionary<Vector3Int, Chunk> Chunks;

    private Queue<ChunkLoading> _chunkLoadingQueue;

    public World(Seed seed)
    {
        Seed = seed;
        Chunks = [];
    }


    public void RecalculateChunks(Vector3Int observerPosition, int viewRadius)
    {
        var extraChunks = GetExtraChunks(observerPosition, viewRadius);
        var newChunksPositions = GetNewChunkPositions(observerPosition, extraChunks);
        DeleteExtraChunks(extraChunks);
    }


    private List<Chunk> GetExtraChunks(Vector3Int observerPosition, int viewRadius)
    {
        var extraChunks = new List<Chunk>();
        foreach (var chunk in Chunks)
        {
            var distanceSquared = Vector3Int.DistanceSquared(observerPosition, chunk.Key);
            if (distanceSquared > viewRadius * viewRadius)
            {
                extraChunks.Add(chunk.Value);
            }
        }

        return extraChunks;
    }

    private HashSet<Vector3Int> GetNewChunkPositions(Vector3Int observerPosition, List<Chunk> oldChunks)
    {
        var newChunksPositions = new HashSet<Vector3Int>();
        foreach (var chunk in oldChunks)
        {
            var newPosition = 2 * observerPosition - chunk.Position;
            newChunksPositions.Add(newPosition);
        }

        return newChunksPositions;
    }

    private void DeleteExtraChunks(List<Chunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            Chunks.Remove(chunk.Position);
            chunk.Dispose();
        }

        chunks.Clear();
    }

    private void EnqueueChunksOnLoad(HashSet<Vector3Int> newChunksPositions)
    {
        foreach (var position in newChunksPositions)
        {
            _chunkLoadingQueue.Enqueue(new ChunkLoading(position));
        }
    }


    // .....
    
    
    public Chunk GetChunk(Vector3Int position)
    {
        var chunkPosition = ChunkMath.GetChunkPosition(position);
        var chunk = Chunks[chunkPosition];

        return chunk;
    }

    public Voxel GetVoxel(Vector3Int position)
    {
        var chunkPosition = ChunkMath.GetChunkPosition(position);
        var chunk = Chunks[chunkPosition];
        var voxelPosition = position - chunkPosition * Chunk.ChunkSize;
        var voxel = chunk.GetVoxel(voxelPosition);

        return voxel;
    }
}