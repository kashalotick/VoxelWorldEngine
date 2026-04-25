using System.Collections.Concurrent;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Core.Threading;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace GameApp.Content.Systems;

public class ChunkUpdateSystem : IDisposable
{
    private readonly VoxelWorld _voxelWorld;
    private readonly DynamicWorkerPool<ChunkTask> _workerPool;
    private readonly HashSet<Vector3Int> _inProgress = new();
    private readonly ConcurrentQueue<ChunkTask> _readyMeshes = new();

    public ChunkUpdateSystem(VoxelWorld voxelWorld)
    {
        _voxelWorld = voxelWorld;
        _workerPool = new DynamicWorkerPool<ChunkTask>(
            minWorkers: 1,
            maxWorkers: 1,
            queueTriggerSize: 999,
            processTask: ProcessChunk
        );
    }

    public void Update(double deltaTime)
    {
        _workerPool.UpdateScaling();

        // GL виклики — тільки тут, в main thread
        while (_readyMeshes.TryDequeue(out var task))
        {
            _voxelWorld.UpdateChunk(task.Chunk);
        }

        var dirty = _voxelWorld.MeshDirtyChunks.ToList();
        _voxelWorld.ClearMeshDirty();

        foreach (var chunkPos in dirty)
        {
            if (!_voxelWorld.Chunks.TryGetValue(chunkPos, out var chunk)) continue;
            lock (_inProgress)
            {
                if (!_inProgress.Add(chunkPos)) continue;
            }
            _workerPool.Enqueue(new ChunkTask(chunkPos, chunk));
        }
    }

    private void ProcessChunk(ChunkTask task)
    {
        try
        {
            var mesh = new MeshBuilder().Build(task.Chunk.Octree);
            task.Chunk.ChunkMesh = mesh;
            _readyMeshes.Enqueue(task); // кладемо для main thread
        }
        finally
        {
            lock (_inProgress)
            {
                _inProgress.Remove(task.Position);
            }
        }
    }

    public void Dispose() => _workerPool.Dispose();

    private record ChunkTask(Vector3Int Position, Chunk Chunk)
        : IComparable<ChunkTask>
    {
        public int CompareTo(ChunkTask? other)
        {
            if (other is null) return 1;
            var cx = Position.X.CompareTo(other.Position.X);
            if (cx != 0) return cx;
            var cy = Position.Y.CompareTo(other.Position.Y);
            if (cy != 0) return cy;
            return Position.Z.CompareTo(other.Position.Z);
        }
    }
}
public record ChunkTask(Vector3Int Position, Chunk Chunk) 
    : IComparable<ChunkTask>
{
    public int CompareTo(ChunkTask? other)
    {
        if (other is null) return 1;
        var cx = Position.X.CompareTo(other.Position.X);
        if (cx != 0) return cx;
        var cy = Position.Y.CompareTo(other.Position.Y);
        if (cy != 0) return cy;
        return Position.Z.CompareTo(other.Position.Z);
    }

}