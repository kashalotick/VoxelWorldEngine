using System.Numerics;

namespace VoxelWorldEngine.Utils;

public static class VectorHelper
{
    public static Vector3 RotateToNormal(Vector3 vector, Vector3 normal)
    {
        return Vector3.Cross(normal, vector);
    }
}