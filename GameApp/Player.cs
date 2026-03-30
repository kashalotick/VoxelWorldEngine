using OpenTK.Mathematics;

namespace GameApp;

public struct Player
{
    public Vector3 Position;
    public Vector3 ViewDirection;
    public Matrix4 ViewMatrix;
    public int ChunkViewRadius;
    
}