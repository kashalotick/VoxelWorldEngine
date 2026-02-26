using System.Collections.Concurrent;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Systems;

public class ChunkLoadingSystem
{
    private const int ChunkPerFrameLimit = 2;
    private ChunkLoader _chunkLoader;

    private World _world;
    private Vector3Int? _activeChunkPosition;


    public ChunkLoadingSystem(World world)
    {
        _chunkLoader = new ChunkLoader(world);
        _world = world;
    }

    public void Update(Player player)
    {
        UpdateChunks();

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
                RequestNewChunk(chunkPosition);
            }
        }

        var toRemove = _world.Chunks.Keys.Except(shouldBeLoaded).ToHashSet();
        var pendingToCancel = _requestedNewChunks.Except(shouldBeLoaded).ToHashSet();

        _chunksToRemove = toRemove;
        foreach (var pos in pendingToCancel)
        {
            _requestedNewChunks.Remove(pos);
        }
    }

    private HashSet<Vector3Int> _chunksToRemove = new();
    private HashSet<Vector3Int> _requestedNewChunks = new();
    private ConcurrentQueue<Chunk> _readyChunks = new();


    private void RequestNewChunk(Vector3Int chunkPosition)
    {
        _requestedNewChunks.Add(chunkPosition);
        Task.Run(() => LoadNewChunk(chunkPosition));
    }

    private void LoadNewChunk(Vector3Int chunkPosition)
    {
        var chunk = _chunkLoader.Get(chunkPosition);
        _readyChunks.Enqueue(chunk);
    }
}