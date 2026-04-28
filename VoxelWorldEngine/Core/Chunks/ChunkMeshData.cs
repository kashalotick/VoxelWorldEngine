using System.Buffers;
using System.Numerics;
using System.Threading;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace VoxelWorldEngine.DataStructures.Special.Collections.Meshes;

public class ChunkMeshData
{
    public uint[] Indices;      // capacity одразу
    public ChunkVertex[] Vertices;

    public ChunkMeshData(uint[] indices, ChunkVertex[] vertices)
    {
        Indices = indices;
        Vertices = vertices;
    }
}

public sealed class RentedMeshData : IDisposable
{
    private int _disposed;

    public ChunkVertex[] Vertices { get; private set; }
    public uint[] Indices { get; private set; }

    public int VertexCount { get; }
    public int IndexCount { get; }

    public RentedMeshData(ChunkVertex[] vertices, int vertexCount, uint[] indices, int indexCount)
    {
        Vertices = vertices;
        Indices = indices;
        VertexCount = vertexCount;
        IndexCount = indexCount;
    }

    public ReadOnlySpan<ChunkVertex> VerticesSpan => Vertices.AsSpan(0, VertexCount);
    public ReadOnlySpan<uint> IndicesSpan => Indices.AsSpan(0, IndexCount);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        if (Vertices.Length > 0)
        {
            ArrayPool<ChunkVertex>.Shared.Return(Vertices, clearArray: false);
        }

        if (Indices.Length > 0)
        {
            ArrayPool<uint>.Shared.Return(Indices, clearArray: false);
        }

        Vertices = Array.Empty<ChunkVertex>();
        Indices = Array.Empty<uint>();
    }
}

public readonly record struct MeshReadyEvent(
    VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int ChunkCoords,
    RentedMeshData Mesh
);
