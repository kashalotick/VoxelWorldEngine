using System.Threading.Channels;
using GameApp.Graphics.World;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace GameApp.Content.Systems;

public sealed class ChunkPipeline : IDisposable, IChunkDirtySink
{
    private readonly Channel<Vector3Int> _requests;
    private readonly Channel<MeshReadyEvent> _results;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task[] _workers;
    private readonly MeshGenerator _generator = new();
    private readonly VoxelWorld _voxelWorld;
    private readonly HashSet<Vector3Int> _inProgress = new();
    private readonly object _inProgressLock = new();

    public ChunkPipeline(VoxelWorld voxelWorld, int workerCount = 2, int capacity = 256)
    {
        _voxelWorld = voxelWorld;
        _requests = Channel.CreateBounded<Vector3Int>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        _results = Channel.CreateBounded<MeshReadyEvent>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

        _workers = new Task[workerCount];
        for (int i = 0; i < workerCount; i++)
        {
            _workers[i] = Task.Run(() => WorkerLoop(_cts.Token));
        }
    }

    public void MarkDirty(Vector3Int chunkCoords)
    {
        lock (_inProgressLock)
        {
            if (!_inProgress.Add(chunkCoords))
            {
                return;
            }
        }

        if (!_requests.Writer.TryWrite(chunkCoords))
        {
            lock (_inProgressLock)
            {
                _inProgress.Remove(chunkCoords);
            }
        }
    }

    public void RequestUnload(Vector3Int chunkCoords)
    {
        lock (_inProgressLock)
        {
            _inProgress.Remove(chunkCoords);
        }
    }

    public void FlushToGpu(GameWorld world)
    {
        while (_results.Reader.TryRead(out var meshEvent))
        {
            try
            {
                world.UploadMesh(meshEvent.ChunkCoords, meshEvent.Mesh);
            }
            finally
            {
                meshEvent.Mesh.Dispose();
                lock (_inProgressLock)
                {
                    _inProgress.Remove(meshEvent.ChunkCoords);
                }
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _requests.Writer.TryComplete();
        _results.Writer.TryComplete();
        try
        {
            Task.WaitAll(_workers);
        }
        catch (AggregateException)
        {
            // Ignore cancellations.
        }
        _cts.Dispose();
    }

    private async Task WorkerLoop(CancellationToken token)
    {
        var reader = _requests.Reader;
        var writer = _results.Writer;

        while (await reader.WaitToReadAsync(token))
        {
            while (reader.TryRead(out var chunkCoords))
            {
                if (!_voxelWorld.TryGetChunk(chunkCoords, out var chunk))
                {
                    lock (_inProgressLock)
                    {
                        _inProgress.Remove(chunkCoords);
                    }
                    continue;
                }

                var mesh = _generator.Generate(chunk.Octree);
                await writer.WriteAsync(new MeshReadyEvent(chunkCoords, mesh), token);
            }
        }
    }
}