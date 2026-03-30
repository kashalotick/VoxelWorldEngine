using OpenTK.Mathematics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace GameApp;

public static class VectorAdapter
{
    // Vector3 -> Vector3Int
    public static Vector3Int ToVector3Int(this Vector3 v) => new((int)v.X, (int)v.Y, (int)v.Z);
    public static Vector3Int FloorToVector3Int(this Vector3 v) => new(
        (int)MathF.Floor(v.X), (int)MathF.Floor(v.Y), (int)MathF.Floor(v.Z));

    // Vector3Int -> Vector3
    public static Vector3 ToVector3(this Vector3Int v) => new(v.X, v.Y, v.Z);

    // Vector2 -> Vector2Int
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static Vector2Int FloorToVector2Int(this Vector2 v) => new(
        (int)MathF.Floor(v.X), (int)MathF.Floor(v.Y));

    // Vector2Int -> Vector2
    public static Vector2 ToVector2(this Vector2Int v) => new(v.X, v.Y);
    

    public static string FancyString(this Vector3 vector) => $"{vector.X:N2}, {vector.Y:N2}, {vector.Z:N2}"; // TODO: temp
}