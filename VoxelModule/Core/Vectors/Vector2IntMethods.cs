namespace VoxelModule.Core.Vectors;

public partial struct Vector2Int
{
    public float Length()
    {
        var lengthSquared = LengthSquared();
        return (float)Math.Sqrt(lengthSquared);
    }

    public int LengthSquared()
    {
        return Dot(this, this);
    }

    public bool IsInBounds(int size)
    {
        return (uint)X < (uint)size && (uint)Y < (uint)size;
    }

    public bool IsBetween(Core.Vectors.Vector2Int min, Core.Vectors.Vector2Int max)
    {
        return X >= min.X && X < max.X && Y >= min.Y && Y < max.Y && Y >= min.Y && Y < max.Y;
    }


    public static float Distance(Core.Vectors.Vector2Int a, Core.Vectors.Vector2Int b)
    {
        var distanceSquared = DistanceSquared(a, b);
        return MathF.Sqrt(distanceSquared);
    }

    public static int DistanceSquared(Core.Vectors.Vector2Int a, Core.Vectors.Vector2Int b)
    {
        var difference = a - b;
        return Dot(difference, difference);
    }

    public static int Dot(Core.Vectors.Vector2Int a, Core.Vectors.Vector2Int b)
    {
        return a.X * b.X + a.Y * b.Y;
    }


    public static Core.Vectors.Vector2Int Abs(Core.Vectors.Vector2Int value)
    {
        return new Core.Vectors.Vector2Int(Math.Abs(value.X), Math.Abs(value.Y));
    }
}