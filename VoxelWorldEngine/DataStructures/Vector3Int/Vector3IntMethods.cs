namespace VoxelWorldEngine.DataStructures.Vector3Int;

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


    public static float Distance(Vector3Int a, Vector3Int b)
    {
        var distanceSquared = DistanceSquared(a, b);
        return MathF.Sqrt(distanceSquared);
    }

    public static int DistanceSquared(Vector3Int a, Vector3Int b)
    {
        var difference = a - b;
        return Dot(difference, difference);
    }

    public static int Dot(Vector3Int a, Vector3Int b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }


    public static Vector3Int Abs(Vector3Int value)
    {
        return new Vector3Int(
            Math.Abs(value.X),
            Math.Abs(value.Y),
            Math.Abs(value.Z)
        );
    }
}