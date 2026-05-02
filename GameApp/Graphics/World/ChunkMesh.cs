using LearningOpenTK.Core;
using LearningOpenTK.Engine.Meshing;
using VoxelModule.DataStructures.Special.Structures.Vertices;

namespace GameApp.Graphics.World;

public class ChunkMesh : Mesh<ChunkVertex, uint>
{
    public ChunkMesh(ChunkVertex[] vertices, uint[] indices) : base(vertices, indices)
    {
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