using System.Numerics;
using Vector2Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector2Int;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace VoxelWorldEngine.Utils;

public static class VectorExtension
{
    public static Vector3Int ToVector3Int(this Vector3 vector) => new((int)vector.X, (int)vector.Y, (int)vector.Z);
    
    public static Vector3Int FloorToVector3Int(this Vector3 vector) => new(
        (int)MathF.Floor(vector.X),
        (int)MathF.Floor(vector.Y),
        (int)MathF.Floor(vector.Z)
    );

    
    public static Vector2Int ToVector2Int(Vector2 vector) => new((int)vector.X, (int)vector.Y);
    

    public static string FancyString(this Vector3 vector) => $"{vector.X:N2}, {vector.Y:N2}, {vector.Z:N2}";
}