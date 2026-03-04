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
        tMin = float.NegativeInfinity;
        tMax = float.PositiveInfinity;

        for (int axis = 0; axis < 3; axis++)
        {
            float origin = axis == 0 ? ray.Origin.X : axis == 1 ? ray.Origin.Y : ray.Origin.Z;
            float dir = axis == 0 ? ray.Direction.X : axis == 1 ? ray.Direction.Y : ray.Direction.Z;
            float bMin = axis == 0 ? boxMin.X : axis == 1 ? boxMin.Y : boxMin.Z;
            float bMax = axis == 0 ? boxMax.X : axis == 1 ? boxMax.Y : boxMax.Z;

            if (MathF.Abs(dir) < 1e-8f)
            {
                // Промінь паралельний цій осі
                if (origin < bMin || origin > bMax)
                    return false;
            }
            else
            {
                float t1 = (bMin - origin) / dir;
                float t2 = (bMax - origin) / dir;
                if (t1 > t2) (t1, t2) = (t2, t1);
                tMin = MathF.Max(tMin, t1);
                tMax = MathF.Min(tMax, t2);
                if (tMin > tMax)
                    return false;
            }
        }

        return true;
    }
}