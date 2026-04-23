using System.Collections.Concurrent;
using System.Numerics;
using LearningOpenTK.Core.Primitives;
using VoxelWorldEngine;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Systems;

public class ChunkLoadingSystem : ILoadable
{
    private const int ChunkPerFrameLimit = 5;
    private ChunkLoader _chunkLoader;

    private VoxelWorld _voxelWorld;
    private Vector3Int? _activeChunkPosition;


    public ChunkLoadingSystem(VoxelWorld voxelWorld)
    {
        _chunkLoader = new ChunkLoader(voxelWorld);
        _voxelWorld = voxelWorld;
    }


    private double _cooldown = 0f;
    private const float CooldownTime = 0.5f;

    public void Update(double deltaTime, Player player)
    {
        _cooldown -= deltaTime;
        UpdateChunks();
        AdjustWorkers();

        if (_cooldown <= 0f)
        {
            var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
            if (playerChunkPosition != _activeChunkPosition)
            {
                _activeChunkPosition = playerChunkPosition;
                RecalculateChunksAround(player);
            }
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
            _voxelWorld.AddChunk(chunk);
            chunksProcessed++;
        }

        // Console.WriteLine($"chunks processed: {chunksProcessed}");
    }

    private void RemoveExtraChunks()
    {
        foreach (var chunkPosition in _chunksToRemove)
        {
            _voxelWorld.RemoveChunk(chunkPosition);
        }

        _chunksToRemove.Clear();
    }

    private void RecalculateChunksAround(Player player)
    {
        var playerChunkPosition = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        var shouldBeLoaded = new HashSet<Vector3Int>();

        for (int x = -player.ChunkViewRadius; x <= player.ChunkViewRadius; x++)
        for (int y = -player.ChunkViewHeightRadius; y <= player.ChunkViewHeightRadius; y++)
        for (int z = -player.ChunkViewRadius; z <= player.ChunkViewRadius; z++)
        {
            var chunkPosition = playerChunkPosition + new Vector3Int(x, y, z);
            shouldBeLoaded.Add(chunkPosition);

            //
            // var aabb = new AABB(
            //     (Vector3)Chunk.ChunkToGlobal(chunkPosition),
            //     (Vector3)(Chunk.ChunkToGlobal(chunkPosition) + Vector3Int.One * Chunk.ChunkSize)
            // );
            // if (!_frustrum.IsAABBVisible(aabb))
            // {
            //     continue;
            // }

            if (!_voxelWorld.Chunks.ContainsKey(chunkPosition) && !_requestedNewChunks.Contains(chunkPosition))
            {
                _requestedNewChunks.Add(chunkPosition);
                lock (_missingChunks)
                {
                    float dist = Vector3Int.Distance(playerChunkPosition, chunkPosition);
                    _missingChunks.Add(new SortedEntry(dist, chunkPosition));
                    _signal.Release();
                }
                // _missingChunks.Add(chunkPosition);
            }
        }

        var toRemove = _voxelWorld.Chunks.Keys.Except(shouldBeLoaded).ToHashSet();
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
    private const int QueueTriggerSize = 81;

    private long _lastScaleUpTime = 0;
    private long _lastScaleDownTime = 0;
    private const long ScaleUpCooldownMs = 500;
    private const long ScaleDownCooldownMs = 2000;

    public void Load()
    {
        for (int i = 0; i < MinWorkers; i++)
        {
            new Thread(ChunkWorker) { IsBackground = true }.Start();
            Interlocked.Increment(ref _activeWorkers);
        }
    }

    private void AdjustWorkers()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int queueSize = _missingChunks.Count;

        if (queueSize > QueueTriggerSize * _activeWorkers
            && _activeWorkers < MaxWorkers
            && now - _lastScaleUpTime > ScaleUpCooldownMs)
        {
            AddWorker();
            _lastScaleUpTime = now;
        }
        else if (queueSize < 10
                 && _activeWorkers > MinWorkers
                 && now - _lastScaleDownTime > ScaleDownCooldownMs)
        {
            _signal.Release(); // воркер сам вийде
            _lastScaleDownTime = now;
        }
    }

    const int maxChunks = 13 * 13 * 13;

    private void AddWorker()
    {
        Interlocked.Increment(ref _activeWorkers);
        new Thread(ChunkWorker) { IsBackground = true }.Start();
    }
    private CancellationTokenSource _cts = new();
    private volatile bool _disposed = false;

    private void ChunkWorker()
    {
        var chunkLoader = new ChunkLoader(_voxelWorld);
        while (!_disposed)
        {
            try
            {
                _signal.Wait(_cts.Token); // виходить чисто при Cancel
            }
            catch (OperationCanceledException)
            {
                return;
            }
            Vector3Int pos;
            lock (_missingChunks)
            {
                if (_missingChunks.Count == 0)
                {
                    if (_activeWorkers > MinWorkers)
                    {
                        Interlocked.Decrement(ref _activeWorkers);
                        return;
                    }

                    continue;
                }

                var first = _missingChunks.Min;
                _missingChunks.Remove(first);
                pos = first.pos;
            }

            var chunk = chunkLoader.Get(pos);
            _readyChunks.Enqueue(chunk);
        }
    }


    public void Dispose()
    {
        _disposed = true;
        _cts.Cancel();
        _signal.Release(MaxWorkers);
    }
}