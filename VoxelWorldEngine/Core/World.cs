using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Voxels;
using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core;

public class World
{
    public Seed Seed;
    public Dictionary<Vector3Int, Chunk> Chunks;

    // TODO: Queue or pool
    
    
    private ChunkLoader _chunkLoader;

    public World(Seed seed)
    {
        Seed = seed;
        Chunks = [];
        _chunkLoader = new ChunkLoader();
    }


    public void RecalculateChunksAround(Vector3Int observerPosition)
    {
        _chunkLoader.SetObserverPosition(observerPosition);
        
        var extraChunks = _chunkLoader.GetExtra(Chunks);
        
        foreach (var chunk in extraChunks)
        {
            Chunks.Remove(chunk.Position);
        }
        
        var missingChunks = _chunkLoader.GetMissing(Chunks);
        var missingChunksOrdered = missingChunks.OrderBy(chunk => Vector3Int.DistanceSquared(chunk.Position, observerPosition));
        
        foreach (var chunk in missingChunksOrdered)
        {
            chunk.Generate();
            Chunks.Add(chunk.Position, chunk);
        }
    }


    

    // private List<Chunk> GetExtraChunks(Vector3Int observerPosition, int viewRadius)
    // {
    //     var extraChunks = new List<Chunk>();
    //     foreach (var chunk in Chunks)
    //     {
    //         var distanceSquared = Vector3Int.DistanceSquared(observerPosition, chunk.Key);
    //         if (distanceSquared > viewRadius * viewRadius)
    //         {
    //             extraChunks.Add(chunk.Value);
    //         }
    //     }
    //
    //     return extraChunks;
    // }
    //
    // private HashSet<Vector3Int> GetNewChunkPositions(Vector3Int observerPosition, List<Chunk> oldChunks)
    // {
    //     var newChunksPositions = new HashSet<Vector3Int>();
    //     foreach (var chunk in oldChunks)
    //     {
    //         // TODO: find new position
    //         var newPosition = 2 * observerPosition - chunk.Position;
    //         newChunksPositions.Add(newPosition);
    //     }
    //
    //     return newChunksPositions;
    // }
    //
    // private void DeleteExtraChunks(List<Chunk> chunks)
    // {
    //     foreach (var chunk in chunks)
    //     {
    //         Chunks.Remove(chunk.Position);
    //         chunk.Dispose();
    //     }
    //
    //     chunks.Clear();
    // }
    //
    // private void EnqueueChunksOnLoad(HashSet<Vector3Int> newChunksPositions)
    // {
    //     foreach (var position in newChunksPositions)
    //     {
    //         _chunkLoadingQueue.Enqueue(new ChunkLoading(position));
    //     }
    // }
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