using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Raycasting;

public static class Raycaster
{
    public static bool IntersectAABB(
        Ray ray,
        Vector3 boxMin,
        Vector3 boxMax,
        out float tMin,
        out float tMax
    )
    {
        var invDir = ray.InvDirection; // кешуй 1/dir один раз на Ray!

        float tx1 = (boxMin.X - ray.Origin.X) * invDir.X;
        float tx2 = (boxMax.X - ray.Origin.X) * invDir.X;
        float ty1 = (boxMin.Y - ray.Origin.Y) * invDir.Y;
        float ty2 = (boxMax.Y - ray.Origin.Y) * invDir.Y;
        float tz1 = (boxMin.Z - ray.Origin.Z) * invDir.Z;
        float tz2 = (boxMax.Z - ray.Origin.Z) * invDir.Z;

        tMin = MathF.Max(MathF.Max(MathF.Min(tx1, tx2), MathF.Min(ty1, ty2)), MathF.Min(tz1, tz2));
        tMax = MathF.Min(MathF.Min(MathF.Max(tx1, tx2), MathF.Max(ty1, ty2)), MathF.Max(tz1, tz2));

        return tMax >= tMin;
    }
}