using System.Numerics;

namespace VoxelModule.Core.Raycasting;

public static class RaycastUtils
{
    public static bool IntersectAABB(
        Ray ray,
        Vector3 boxMin,
        Vector3 boxMax,
        out float tMin,
        out float tMax,
        out Vector3 normal
    )
    {
        var invDir = ray.InvDirection; // кешуй 1/dir один раз на Ray!

        float tx1 = (boxMin.X - ray.Origin.X) * invDir.X;
        float tx2 = (boxMax.X - ray.Origin.X) * invDir.X;
        float ty1 = (boxMin.Y - ray.Origin.Y) * invDir.Y;
        float ty2 = (boxMax.Y - ray.Origin.Y) * invDir.Y;
        float tz1 = (boxMin.Z - ray.Origin.Z) * invDir.Z;
        float tz2 = (boxMax.Z - ray.Origin.Z) * invDir.Z;

        float tMinX = MathF.Min(tx1, tx2);
        float tMinY = MathF.Min(ty1, ty2);
        float tMinZ = MathF.Min(tz1, tz2);

        tMin = MathF.Max(MathF.Max(tMinX, tMinY), tMinZ);
        tMax = MathF.Min(MathF.Min(MathF.Max(tx1, tx2), MathF.Max(ty1, ty2)), MathF.Max(tz1, tz2));

        if (tMin == tMinX) normal = new Vector3(-MathF.Sign(ray.Direction.X), 0, 0);
        else if (tMin == tMinY) normal = new Vector3(0, -MathF.Sign(ray.Direction.Y), 0);
        else normal = new Vector3(0, 0, -MathF.Sign(ray.Direction.Z));

        return tMax >= tMin;
    }
}