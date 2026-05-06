using System.Numerics;
using GameEngine.Core;
using GameEngine.Engine.Meshing;
using OpenTK.Graphics.OpenGL4;

namespace GameApp.Graphics.VoxelSelectionSystem;

public class CubeEdgeMesh : Mesh<CubeEdgeVertex, uint>
{
    public CubeEdgeMesh(Vector3 color) : base(new CubeEdgeVertex[8], new uint[8])
    {
        Vertices =
        [
            new CubeEdgeVertex(new Vector3(0, 0, 0), color),
            new CubeEdgeVertex(new Vector3(0, 0, 1), color),
            new CubeEdgeVertex(new Vector3(0, 1, 0), color),
            new CubeEdgeVertex(new Vector3(0, 1, 1), color),
            new CubeEdgeVertex(new Vector3(1, 0, 0), color),
            new CubeEdgeVertex(new Vector3(1, 0, 1), color),
            new CubeEdgeVertex(new Vector3(1, 1, 0), color),
            new CubeEdgeVertex(new Vector3(1, 1, 1), color)
        ];

        Indices =
        [
            0, 4, 4, 6, 6, 2, 2, 0,
            1, 5, 5, 7, 7, 3, 3, 1,
            0, 1, 4, 5, 6, 7, 2, 3
        ];
    }


    protected override void DefineAttributePointers()
    {
        new VaoBuilder(SizeOfTVertex)
            .AddFloat(3)
            .AddFloat(3);
    }

    public new void Render()
    {
        if (Vao == 0) return;

        GL.LineWidth(2f);
        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Lines, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}