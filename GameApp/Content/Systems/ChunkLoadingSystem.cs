using System.Collections.Concurrent;
using GameApp.Utils;
using GameEngine.Core.Lifecycle;
using GameEngine.Core.Threading;
using VoxelModule.Core;
using VoxelModule.Core.Serialization;
using VoxelModule.Engine;
using VoxelModule.Engine.Chunks;
using Chunk = VoxelModule.Engine.Chunks.Chunk;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace GameApp.Content.Systems;

public class ChunkLoadingSystem : ILoadable
{
    private const int ChunkPerFrameLimit = 8;
    private const float CooldownTime = 0.5f;
    private readonly ConcurrentQueue<Chunk> _readyChunks = new();
    private readonly HashSet<Vector3Int> _requestedNewChunks = new();

    // Використовуємо ThreadLocal замість звичайного поля
    private readonly ThreadLocal<ChunkFactory> _threadLocalChunkLoader;

    private readonly VoxelWorld _voxelWorld;
    private readonly DynamicWorkerPool<ChunkLoadTask> _workerPool;

    private Vector3Int? _activeChunkPosition;

    private HashSet<Vector3Int> _chunksToRemove = new();
    private double _cooldown;

    public ChunkLoadingSystem(VoxelWorld voxelWorld, IChunkMementoRepository chunkRepository)
    {
        _voxelWorld = voxelWorld;

        // Ініціалізуємо ThreadLocal. Він викличе лямбду ТІЛЬКИ тоді, 
        // коли новий потік вперше звернеться до Value.
        _threadLocalChunkLoader
            = new ThreadLocal<ChunkFactory>(() => new ChunkFactory(chunkRepository, _voxelWorld.Seed));

        _workerPool = new DynamicWorkerPool<ChunkLoadTask>(
            2,
            8,
            81,
            BuildChunkInBackground
        );
    }

    public void Load()
    {
    }

    public void Dispose()
    {
        _workerPool.Dispose();
        _threadLocalChunkLoader.Dispose(); // Не забуваємо чистити
    }

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
        // Беремо екземпляр ChunkGenerateStrategy, який належить КОНКРЕТНО ЦЬОМУ потоку
        var chunkLoader = _threadLocalChunkLoader.Value;
        var chunk = chunkLoader.GetChunk(task.Position);

        _readyChunks.Enqueue(chunk);
    }

    private void RecalculateChunksAround(Player player)
    {
        // ... (код залишився без змін, такий як у попередньому повідомленні) ...
        var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        var shouldBeLoaded = new HashSet<Vector3Int>();

        for (var x = -player.ChunkViewRadius; x <= player.ChunkViewRadius; x++)
        for (var y = -player.ChunkViewHeightRadius; y <= player.ChunkViewHeightRadius; y++)
        for (var z = -player.ChunkViewRadius; z <= player.ChunkViewRadius; z++)
        {
            var chunkPosition = playerChunkPosition + new Vector3Int(x, y, z);
            shouldBeLoaded.Add(chunkPosition);

            if (!_voxelWorld.Chunks.ContainsKey(chunkPosition) && !_requestedNewChunks.Contains(chunkPosition))
            {
                _requestedNewChunks.Add(chunkPosition);
                var dist = Vector3Int.Distance(playerChunkPosition, chunkPosition);
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
        var chunksProcessed = 0;
        while (chunksProcessed < ChunkPerFrameLimit && _readyChunks.TryDequeue(out var chunk))
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
    
    public Chunk LoadChunkImmediately(Vector3Int position)
    {
        if (_voxelWorld.Chunks.TryGetValue(position, out var existingChunk))
        {
            return existingChunk;
        }

        var chunkLoader = _threadLocalChunkLoader.Value;
        var chunk = chunkLoader.GetChunk(position);

        _voxelWorld.AddChunk(chunk);
        _requestedNewChunks.Remove(position);
        _workerPool.RemoveWhere(task => task.Position == position);

        return chunk;
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
        var cmp = Distance.CompareTo(other.Distance);
        if (cmp != 0) return cmp;
        cmp = Position.X.CompareTo(other.Position.X);
        if (cmp != 0) return cmp;
        cmp = Position.Y.CompareTo(other.Position.Y);
        if (cmp != 0) return cmp;
        return Position.Z.CompareTo(other.Position.Z);
    }
}