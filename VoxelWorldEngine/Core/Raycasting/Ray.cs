using System.Numerics;

namespace VoxelWorldEngine.Core.Raycasting;

public struct Ray
{
    public float Length;
    public Vector3 Origin;
    public Vector3 Direction
    {
        get;
        set
        {
            field = value;
            InvDirection = new Vector3(1f / value.X, 1f / value.Y, 1f / value.Z);
        }
    }
    public Vector3 InvDirection;
    
    public override string ToString()
    {
        return $"Ray | Origin: {Origin}, Dir: {Direction}, InvDir: {InvDirection}, Length: {Length}";
    }
}

