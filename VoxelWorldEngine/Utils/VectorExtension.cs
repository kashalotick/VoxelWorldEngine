using System.Numerics;
using Vector2Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector2Int;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace VoxelWorldEngine.Utils;

public static class VectorExtension
{
    public static Vector3Int ToVector3Int(this Vector3 vector) => new((int)vector.X, (int)vector.Y, (int)vector.Z);
    public static Vector2Int ToVector2Int(Vector2 vector) => new((int)vector.X, (int)vector.Y);

    public static Vector3 RoundTo(this Vector3 vector, int accuracy) => new(
        (float)Math.Round(vector.X, accuracy),
        (float)Math.Round(vector.X, accuracy),
        (float)Math.Round(vector.X, accuracy)
    );

    public static string FancyString(this Vector3 vector) => $"{vector.X}, {vector.Y}, {vector.Z}";
}