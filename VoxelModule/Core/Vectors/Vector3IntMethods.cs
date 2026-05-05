namespace VoxelModule.Core.Vectors;

public partial struct Vector3Int
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
        return (uint)X < (uint)size && (uint)Y < (uint)size && (uint)Z < (uint)size;
    }

    public bool IsBetween(Core.Vectors.Vector3Int min, Core.Vectors.Vector3Int max)
    {
        return X >= min.X && X < max.X && Y >= min.Y && Y < max.Y && Y >= min.Y && Y < max.Y;
    }


    public static float Distance(Core.Vectors.Vector3Int a, Core.Vectors.Vector3Int b)
    {
        var distanceSquared = DistanceSquared(a, b);
        return MathF.Sqrt(distanceSquared);
    }

    public static int DistanceSquared(Core.Vectors.Vector3Int a, Core.Vectors.Vector3Int b)
    {
        var difference = a - b;
        return Dot(difference, difference);
    }

    public static int Dot(Core.Vectors.Vector3Int a, Core.Vectors.Vector3Int b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static Core.Vectors.Vector3Int Abs(Core.Vectors.Vector3Int value)
    {
        return new Core.Vectors.Vector3Int(Math.Abs(value.X), Math.Abs(value.Y), Math.Abs(value.Z));
    }
}