using LearningOpenTK.Engine.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace GameApp.Content;

public class ChunkMesh : Mesh<ChunkVertex, uint>
{
    public ChunkMesh(ChunkVertex[] vertices, uint[] indices) : base(vertices, indices)
    {
    }

    protected override void DefineAttributePointers()
    {
        // position
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeOfTVertex, 0);
        GL.EnableVertexAttribArray(0);
        // normal
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, SizeOfTVertex, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        // uv
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, SizeOfTVertex, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        // block id
        GL.VertexAttribIPointer(3, 1, VertexAttribIntegerType.UnsignedByte, SizeOfTVertex, (nint)(8 * sizeof(float)));
        GL.EnableVertexAttribArray(3);
    }
}