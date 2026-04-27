using System.Numerics;
using System.Runtime.InteropServices;

namespace GameApp.Graphics.VoxelSelectionSystem;

[StructLayout(LayoutKind.Sequential)]
public struct CubeEdgeVertex
{
    public Vector3 Position;
    public Vector3 Color;

    public CubeEdgeVertex(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}