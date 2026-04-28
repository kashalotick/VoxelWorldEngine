using LearningOpenTK.Core;
using LearningOpenTK.Engine.Meshes;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace GameApp.Graphics.World;

public class ChunkMesh : Mesh<ChunkVertex, uint>
{
    private int _vertexCapacityBytes;
    private int _indexCapacityBytes;
    private int _indexCount;

    public ChunkMesh() : base(Array.Empty<ChunkVertex>(), Array.Empty<uint>())
    {
    }

    private void EnsureGpuObjects()
    {
        if (Vao != 0) return;

        Vao = GL.GenVertexArray();
        GL.BindVertexArray(Vao);

        Vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

        Ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);

        DefineAttributePointers();
        GL.BindVertexArray(0);
    }

    public unsafe void Upload(ReadOnlySpan<ChunkVertex> vertices, ReadOnlySpan<uint> indices)
    {
        EnsureGpuObjects();

        _indexCount = indices.Length;
        if (_indexCount == 0)
        {
            return;
        }

        GL.BindVertexArray(Vao);

        var vertexBytes = vertices.Length * SizeOfTVertex;
        GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
        if (vertexBytes > _vertexCapacityBytes)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            _vertexCapacityBytes = vertexBytes;
        }

        fixed (ChunkVertex* vertexPtr = vertices)
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertexBytes, (IntPtr)vertexPtr);
        }

        var indexBytes = indices.Length * sizeof(uint);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
        if (indexBytes > _indexCapacityBytes)
        {
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            _indexCapacityBytes = indexBytes;
        }

        fixed (uint* indexPtr = indices)
        {
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, indexBytes, (IntPtr)indexPtr);
        }

        GL.BindVertexArray(0);
    }

    public override void Render()
    {
        if (Vao == 0 || _indexCount == 0) return;

        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    }

    protected override void DefineAttributePointers()
    {
        // position
        // normal
        // uv
        // block id
        new VaoBuilder(SizeOfTVertex)
            .AddFloat(3)
            .AddFloat(3)
            .AddFloat(2)
            .AddUByte(1);
        // // position
        // GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeOfTVertex, 0);
        // GL.EnableVertexAttribArray(0);
        // // normal
        // GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, SizeOfTVertex, 3 * sizeof(float));
        // GL.EnableVertexAttribArray(1);
        // // uv
        // GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, SizeOfTVertex, 6 * sizeof(float));
        // GL.EnableVertexAttribArray(2);
        // // block id
        // GL.VertexAttribIPointer(3, 1, VertexAttribIntegerType.UnsignedByte, SizeOfTVertex, (nint)(8 * sizeof(float)));
        // GL.EnableVertexAttribArray(3);
    }
}