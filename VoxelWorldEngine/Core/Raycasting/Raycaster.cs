using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core.Raycasting;

public static class Raycaster
{
    public static bool IntersectsAABB(
        Ray ray,
        Vector3Int min,
        Vector3Int max,
        out float tIn,
        out float tOut
    )
    {
        var invDir = Vector3.One / ray.Direction;

        var t1 = ((Vector3)min - ray.Origin) * invDir;
        var t2 = ((Vector3)max + Vector3.One - ray.Origin) * invDir;

        var tMin = Vector3.Min(t1, t2);
        var tMax = Vector3.Max(t1, t2);

        tIn = MathF.Max(MathF.Max(tMin.X, tMin.Y), tMin.Z);
        tOut = MathF.Min(MathF.Min(tMax.X, tMax.Y), tMax.Z);

        return tIn <= tOut && tOut >= 0 && tIn <= ray.Length;
    }
}