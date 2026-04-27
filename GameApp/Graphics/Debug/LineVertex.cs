using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace GameApp.Graphics.Debug;

[StructLayout(LayoutKind.Sequential)]
public struct LineVertex
{
    public Vector3 Position;
    public Vector3 Color;

    public LineVertex(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}