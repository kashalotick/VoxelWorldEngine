using System.Collections.Concurrent;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Core.Threading;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace GameApp.Content.Systems;

public class ChunkLoadingSystem : ILoadable
{
    private const int ChunkPerFrameLimit = 5;
    private const float CooldownTime = 0.5f;

    private readonly VoxelWorld _voxelWorld;
    private readonly DynamicWorkerPool<ChunkLoadTask> _workerPool;
    
    // Використовуємо ThreadLocal замість звичайного поля
    private readonly ThreadLocal<ChunkLoader> _threadLocalChunkLoader; 
    
    private Vector3Int? _activeChunkPosition;
    private double _cooldown = 0f;

    private HashSet<Vector3Int> _chunksToRemove = new();
    private HashSet<Vector3Int> _requestedNewChunks = new();
    private ConcurrentQueue<Chunk> _readyChunks = new();

    public ChunkLoadingSystem(VoxelWorld voxelWorld)
    {
        _voxelWorld = voxelWorld;
        
        // Ініціалізуємо ThreadLocal. Він викличе лямбду ТІЛЬКИ тоді, 
        // коли новий потік вперше звернеться до Value.
        _threadLocalChunkLoader = new ThreadLocal<ChunkLoader>(() => new ChunkLoader(_voxelWorld));
        
        _workerPool = new DynamicWorkerPool<ChunkLoadTask>(
            minWorkers: 2, 
            maxWorkers: 8, 
            queueTriggerSize: 81, 
            processTask: BuildChunkInBackground
        );
    }

    public void Load() { }

    public void Update(double deltaTime, Player player)
    {
        _cooldown -= deltaTime;
        
        UpdateChunks();
        _workerPool.UpdateScaling();

        if (_cooldown <= 0f)
        {
            var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
            if (playerChunkPosition != _activeChunkPosition)
            {
                _activeChunkPosition = playerChunkPosition;
                RecalculateChunksAround(player);
            }
            _cooldown = CooldownTime;
        }
    }

    private void BuildChunkInBackground(ChunkLoadTask task)
    {
        // Беремо екземпляр ChunkLoader, який належить КОНКРЕТНО ЦЬОМУ потоку
        var chunkLoader = _threadLocalChunkLoader.Value;
        var chunk = chunkLoader.BuildChunk(task.Position);
        
        _readyChunks.Enqueue(chunk);
    }

    private void RecalculateChunksAround(Player player)
    {
        // ... (код залишився без змін, такий як у попередньому повідомленні) ...
        var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        var shouldBeLoaded = new HashSet<Vector3Int>();

        for (int x = -player.ChunkViewRadius; x <= player.ChunkViewRadius; x++)
        for (int y = -player.ChunkViewHeightRadius; y <= player.ChunkViewHeightRadius; y++)
        for (int z = -player.ChunkViewRadius; z <= player.ChunkViewRadius; z++)
        {
            var chunkPosition = playerChunkPosition + new Vector3Int(x, y, z);
            shouldBeLoaded.Add(chunkPosition);

            if (!_voxelWorld.Chunks.ContainsKey(chunkPosition) && !_requestedNewChunks.Contains(chunkPosition))
            {
                _requestedNewChunks.Add(chunkPosition);
                float dist = Vector3Int.Distance(playerChunkPosition, chunkPosition);
                _workerPool.Enqueue(new ChunkLoadTask(dist, chunkPosition));
            }
        }

        var toRemove = _voxelWorld.Chunks.Keys.Except(shouldBeLoaded).ToHashSet();
        var pendingToCancel = _requestedNewChunks.Except(shouldBeLoaded).ToHashSet();

        _chunksToRemove = toRemove;
        
        foreach (var pos in pendingToCancel)
        {
            _requestedNewChunks.Remove(pos);
            _workerPool.RemoveWhere(task => task.Position == pos);
        }
    }

    private void UpdateChunks()
    {
        // ... (код залишився без змін) ...
        int chunksProcessed = 0;
        while (chunksProcessed < ChunkPerFrameLimit && _readyChunks.TryDequeue(out Chunk chunk))
        {
            if (!_requestedNewChunks.Contains(chunk.Position)) continue;

            _requestedNewChunks.Remove(chunk.Position);
            _voxelWorld.AddChunk(chunk);
            chunksProcessed++;
        }

        foreach (var chunkPosition in _chunksToRemove)
        {
            _voxelWorld.RemoveChunk(chunkPosition);
        }
        _chunksToRemove.Clear();
    }

    public void Dispose()
    {
        _workerPool.Dispose();
        _threadLocalChunkLoader.Dispose(); // Не забуваємо чистити
    }
}

public struct ChunkLoadTask : IComparable<ChunkLoadTask>
{
    public float Distance { get; }
    public Vector3Int Position { get; }

    public ChunkLoadTask(float distance, Vector3Int position)
    {
        Distance = distance;
        Position = position;
    }

    public int CompareTo(ChunkLoadTask other)
    {
        int cmp = Distance.CompareTo(other.Distance);
        if (cmp != 0) return cmp;
        cmp = Position.X.CompareTo(other.Position.X);
        if (cmp != 0) return cmp;
        cmp = Position.Y.CompareTo(other.Position.Y);
        if (cmp != 0) return cmp;
        return Position.Z.CompareTo(other.Position.Z);
    }
}