using System.Numerics;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Utils;

public static class VectorExtension
{
    public static Vector3Int ToVector3Int(this Vector3 vector) => new ((int)vector.X, (int)vector.Y, (int)vector.Z);
    public static Vector2Int Vector2Int(Vector2 vector) => new ((int)vector.X, (int)vector.Y);
}