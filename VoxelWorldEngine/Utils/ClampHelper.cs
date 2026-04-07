using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Utils;

public static class ClampHelper
{
    public static Vector3Int ClampVector3(Vector3Int v, Vector3Int min, Vector3Int maX)
    {
        return new Vector3Int(
            Math.Clamp(v.X, min.X, maX.X),
            Math.Clamp(v.Y, min.Y, maX.Y),
            Math.Clamp(v.Z, min.Z, maX.Z)
        );
    }
    public static Vector3 ClampVector3(Vector3 v, Vector3 min, Vector3 maX)
    {
        return new Vector3(
            Math.Clamp(v.X, min.X, maX.X),
            Math.Clamp(v.Y, min.Y, maX.Y),
            Math.Clamp(v.Z, min.Z, maX.Z)
        );
    }
    
    public static Vector2Int ClampVector2(Vector2Int v, Vector2Int min, Vector2Int maX)
    {
        return new Vector2Int(
            Math.Clamp(v.X, min.X, maX.X),
            Math.Clamp(v.Y, min.Y, maX.Y)
        );
    }
    public static Vector2 ClampVector2(Vector2 v, Vector2 min, Vector2 maX)
    {
        return new Vector2(
            Math.Clamp(v.X, min.X, maX.X),
            Math.Clamp(v.Y, min.Y, maX.Y)
        );
    }
}