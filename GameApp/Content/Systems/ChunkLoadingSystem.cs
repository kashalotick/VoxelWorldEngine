using System.Collections.Concurrent;
using System.Numerics;
using VoxelWorldEngine;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Systems;

public class ChunkLoadingSystem : ISystem
{
    private const int ChunkPerFrameLimit = 100;
    private ChunkLoader _chunkLoader;

    private World _world;
    private Vector3Int? _activeChunkPosition;


    private Frustrum _frustrum = new Frustrum();

    public ChunkLoadingSystem(World world)
    {
        _chunkLoader = new ChunkLoader(world);
        _world = world;
    }

    public void Initialize()
    {
        // TODO: Proxy???
        StartWorkers();
    }

    public void Update(Player player)
    {
        UpdateChunks();
        AdjustWorkers();

        var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        if (playerChunkPosition != _activeChunkPosition)
        {
            _activeChunkPosition = playerChunkPosition;
            RecalculateChunksAround(player);
        }
    }

    public void UpdateChunks()
    {
        AddNewChunks();
        RemoveExtraChunks();
    }

    private void AddNewChunks()
    {
        int chunksProcessed = 0;

        while (chunksProcessed < ChunkPerFrameLimit && _readyChunks.TryDequeue(out Chunk chunk))
        {
            if (!_requestedNewChunks.Contains(chunk.Position)) continue;

            _requestedNewChunks.Remove(chunk.Position);
            _world.AddChunk(chunk);
            chunksProcessed++;
        }
    }

    private void RemoveExtraChunks()
    {
        foreach (var chunkPosition in _chunksToRemove)
        {
            _world.RemoveChunk(chunkPosition);
        }

        _chunksToRemove.Clear();
    }

    private void RecalculateChunksAround(Player player)
    {
        var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        var shouldBeLoaded = new HashSet<Vector3Int>();

        for (int x = -player.ChunkViewRadius; x <= player.ChunkViewRadius; x++)
        for (int y = -player.ChunkViewRadius; y <= player.ChunkViewRadius; y++)
        for (int z = -player.ChunkViewRadius; z <= player.ChunkViewRadius; z++)
        {
            var chunkPosition = playerChunkPosition + new Vector3Int(x, y, z);
            shouldBeLoaded.Add(chunkPosition);
            
            
            if (!_world.Chunks.ContainsKey(chunkPosition) && !_requestedNewChunks.Contains(chunkPosition))
            {
                _requestedNewChunks.Add(chunkPosition);
                lock (_missingChunks)
                {
                    Counter.Increment(CounterType.MissingChunksTryAdd);

                    float dist = Vector3Int.Distance(playerChunkPosition, chunkPosition);
                    _missingChunks.Add(new SortedEntry(dist, chunkPosition));
                    _signal.Release();
                }
                // _missingChunks.Add(chunkPosition);
            }
        }

        var toRemove = _world.Chunks.Keys.Except(shouldBeLoaded).ToHashSet();
        var pendingToCancel = _requestedNewChunks.Except(shouldBeLoaded).ToHashSet();

        _chunksToRemove = toRemove;
        foreach (var pos in pendingToCancel)
        {
            _requestedNewChunks.Remove(pos);
            lock (_missingChunks)
            {
                _missingChunks.RemoveWhere(e => e.pos == pos);
            }

            // _missingChunks.TryTake(out var chunk);
        }

        // _requestedNewChunks.Clear();
    }

    private HashSet<Vector3Int> _chunksToRemove = new();
    private HashSet<Vector3Int> _requestedNewChunks = new();
    private ConcurrentQueue<Chunk> _readyChunks = new();

    private struct SortedEntry : IComparable<SortedEntry>
    {
        public float distance;
        public Vector3Int pos;

        public SortedEntry(float distance, Vector3Int pos)
        {
            this.distance = distance;
            this.pos = pos;
        }

        public int CompareTo(SortedEntry other)
        {
            int cmp = distance.CompareTo(other.distance);
            if (cmp != 0) return cmp;
            cmp = pos.X.CompareTo(other.pos.X);
            if (cmp != 0) return cmp;
            cmp = pos.Y.CompareTo(other.pos.Y);
            if (cmp != 0) return cmp;
            return pos.Z.CompareTo(other.pos.Z);
        }
    }

    private SortedSet<SortedEntry> _missingChunks = new();
    private SemaphoreSlim _signal = new(0);


    private int _activeWorkers = 0;
    private const int MaxWorkers = 8;
    private const int MinWorkers = 2;
    private const int QueueTriggerSize = 100; // TODO: player view radius depending

    private void StartWorkers()
    {
        for (int i = 0; i < MinWorkers; i++)
        {
            new Thread(ChunkWorker) { IsBackground = true }.Start();
            Interlocked.Increment(ref _activeWorkers);
        }
    }

    private void AdjustWorkers()
    {
        int queueSize = _missingChunks.Count;
        if (queueSize > QueueTriggerSize * _activeWorkers && _activeWorkers < MaxWorkers)
        {
            AddWorker();
        }
        else if (queueSize < 10 && _activeWorkers > MinWorkers)
        {
            // worker сам зупиниться
            _signal.Release(); // розбудити щоб вийшов
        }
    }

    private void AddWorker()
    {
        Console.WriteLine($"Add worker: {_activeWorkers}");

        Interlocked.Increment(ref _activeWorkers);
        new Thread(ChunkWorker) { IsBackground = true }.Start();
    }

    private void ChunkWorker()
    {
        while (true)
        {
            _signal.Wait();
            Vector3Int pos;
            lock (_missingChunks)
            {
                if (_missingChunks.Count == 0)
                {
                    if (_activeWorkers > MinWorkers)
                    {
                        Console.WriteLine($"Remove worker: {_activeWorkers}");

                        Interlocked.Decrement(ref _activeWorkers);
                        return;
                    }

                    continue;
                }

                Counter.Increment(CounterType.MissingChunks);
                var first = _missingChunks.Min;
                _missingChunks.Remove(first);
                pos = first.pos;
            }

            var chunk = _chunkLoader.Get(pos);
            _readyChunks.Enqueue(chunk);
        }
    }


    public void Dispose()
    {
        Counter.Display();
        _signal.Dispose();
        // _missingChunks.CompleteAdding();
        // _missingChunks.Dispose();
    }
}