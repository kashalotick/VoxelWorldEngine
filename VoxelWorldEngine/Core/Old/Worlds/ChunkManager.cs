using System.Collections.Concurrent;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace VoxelWorldEngine.Core.Worlds;

public class ChunkManager
{
    private Dictionary<Vector3Int, Chunk> _chunks;
    private ConcurrentQueue<Chunk> _readyChunks;
    private HashSet<Vector3Int> _requestedChunks;
    private ChunkBuilder _chunkBuilder;
    
    private Observer _previousObserver;

    public ChunkManager()
    {
        _chunks = new Dictionary<Vector3Int, Chunk>();
        _readyChunks = new ConcurrentQueue<Chunk>();
        _requestedChunks = new HashSet<Vector3Int>();
        _chunkBuilder = new ChunkBuilder(1234);
    }


    
    public void Update(double deltaTime, Observer observer)
    {
        LoadChunks();
        if (_previousObserver != observer)
        {
            _previousObserver = observer;
            UpdateVisibleChunks(observer);
        }
    }


    private void UpdateVisibleChunks(Observer observer)
    {
        var viewRadiusVector = new Vector3Int(observer.ViewRadius); 
        (Vector3Int min, Vector3Int max) aabb = (observer.Position - viewRadiusVector, observer.Position + viewRadiusVector);

        for (int x = aabb.min.X; x < aabb.max.X; x++)
        for (int y = aabb.min.Y; y < aabb.max.Y; y++)
        for (int z = aabb.min.Z; z < aabb.max.Z; z++)
        {
            var chunkPosition = new Vector3Int(x, y, z);
            RequestChunkGeneration(chunkPosition);
        }
    }

    private void LoadChunks()
    {
        const int chunkPerFrameLimit = 5;
        int chunksProcessed = 0;
        
        while (_readyChunks.TryDequeue(out Chunk chunk) && chunksProcessed < chunkPerFrameLimit) 
        {
            _requestedChunks.Remove(chunk.Position);
            LoadChunk(chunk);
            chunksProcessed++;
        }
    }

    private void LoadChunk(Chunk chunk)
    {
        _chunks.TryAdd(chunk.Position, chunk);
    }

    private bool NeedToRequestChunk(Vector3Int chunkPosition)
    {
        return !_chunks.ContainsKey(chunkPosition) && !_requestedChunks.Contains(chunkPosition);
    }
    private void RequestChunkGeneration(Vector3Int chunkPosition)
    {
        if (NeedToRequestChunk(chunkPosition))
        {
            _requestedChunks.Add(chunkPosition);

            Task.Run(() => GenerateChunk(chunkPosition));
        }
    }
    private void GenerateChunk(Vector3Int chunkPosition)
    {
        var chunkBuilder = new ChunkBuilder(1234);

        var chunk = chunkBuilder.Build(chunkPosition);
        _readyChunks.Enqueue(chunk);
    }
}