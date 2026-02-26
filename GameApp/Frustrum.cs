using System.Numerics;

namespace GameApp;

public struct AABB
{
    public Vector3 Min;
    public Vector3 Max;

    public AABB(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public Vector3 Center => (Min + Max) * 0.5f;
    public Vector3 Extents => (Max - Min) * 0.5f;
}

public struct Plane
{
    public Vector3 Normal;
    public float Distance;

    public Plane(Vector3 normal, float distance)
    {
        Normal = Vector3.Normalize(normal);
        Distance = distance;
    }

    // Відстань від точки до площини
    public float GetSignedDistance(Vector3 point)
        => Vector3.Dot(Normal, point) + Distance;
}


public class Frustrum
{
     // 0=Left, 1=Right, 2=Bottom, 3=Top, 4=Near, 5=Far
    private readonly Plane[] _planes = new Plane[6];

    /// <summary>
    /// Будує frustum з матриці ViewProjection
    /// </summary>
    public void ExtractFromMatrix(Matrix4x4 viewProjection)
    {
        // Left
        _planes[0] = NormalizePlane(
            viewProjection.M14 + viewProjection.M11,
            viewProjection.M24 + viewProjection.M21,
            viewProjection.M34 + viewProjection.M31,
            viewProjection.M44 + viewProjection.M41);

        // Right
        _planes[1] = NormalizePlane(
            viewProjection.M14 - viewProjection.M11,
            viewProjection.M24 - viewProjection.M21,
            viewProjection.M34 - viewProjection.M31,
            viewProjection.M44 - viewProjection.M41);

        // Bottom
        _planes[2] = NormalizePlane(
            viewProjection.M14 + viewProjection.M12,
            viewProjection.M24 + viewProjection.M22,
            viewProjection.M34 + viewProjection.M32,
            viewProjection.M44 + viewProjection.M42);

        // Top
        _planes[3] = NormalizePlane(
            viewProjection.M14 - viewProjection.M12,
            viewProjection.M24 - viewProjection.M22,
            viewProjection.M34 - viewProjection.M32,
            viewProjection.M44 - viewProjection.M42);

        // Near
        _planes[4] = NormalizePlane(
            viewProjection.M13,
            viewProjection.M23,
            viewProjection.M33,
            viewProjection.M43);

        // Far
        _planes[5] = NormalizePlane(
            viewProjection.M14 - viewProjection.M13,
            viewProjection.M24 - viewProjection.M23,
            viewProjection.M34 - viewProjection.M33,
            viewProjection.M44 - viewProjection.M43);
    }

    private static Plane NormalizePlane(float a, float b, float c, float d)
    {
        float length = MathF.Sqrt(a * a + b * b + c * c);
        return new Plane(
            new Vector3(a / length, b / length, c / length),
            d / length);
    }

    /// <summary>
    /// Перевіряє чи AABB перетинає frustum.
    /// Повертає false якщо AABB повністю за межами frustum.
    /// </summary>
    public bool IsAABBVisible(in AABB aabb)
    {
        Vector3 center = aabb.Center;
        Vector3 extents = aabb.Extents;

        foreach (var plane in _planes)
        {
            // Проекція extents на нормаль площини (радіус ефективного "сфери")
            float r = extents.X * MathF.Abs(plane.Normal.X)
                    + extents.Y * MathF.Abs(plane.Normal.Y)
                    + extents.Z * MathF.Abs(plane.Normal.Z);

            float signedDist = plane.GetSignedDistance(center);

            // Якщо центр + радіус < 0 — AABB повністю поза цією площиною
            if (signedDist + r < 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Повертає детальний результат перевірки
    /// </summary>
    public FrustumIntersection ClassifyAABB(in AABB aabb)
    {
        Vector3 center = aabb.Center;
        Vector3 extents = aabb.Extents;
        bool fullyInside = true;

        foreach (var plane in _planes)
        {
            float r = extents.X * MathF.Abs(plane.Normal.X)
                    + extents.Y * MathF.Abs(plane.Normal.Y)
                    + extents.Z * MathF.Abs(plane.Normal.Z);

            float signedDist = plane.GetSignedDistance(center);

            if (signedDist + r < 0)
                return FrustumIntersection.Outside;

            if (signedDist - r < 0)
                fullyInside = false;
        }

        return fullyInside ? FrustumIntersection.Inside : FrustumIntersection.Intersects;
    }
}

public enum FrustumIntersection
{
    Outside,
    Intersects,
    Inside
}