namespace VoxelWorldEngine.DataStructures.Vector2Int;

public partial struct Vector2Int
{
    public float Length()
    {
        var lengthSquared = LengthSquared();
        return (float)Math.Sqrt(lengthSquared);
    }

    public int LengthSquared() => Dot(this, this);
    public bool IsInBounds(int size) => (uint)X < (uint)size && (uint)Y < (uint)size;

    public bool IsBetween(Vector2Int min, Vector2Int max) =>
        X >= min.X && X < max.X && Y >= min.Y && Y < max.Y && Y >= min.Y && Y < max.Y;


    public static float Distance(Vector2Int a, Vector2Int b)
    {
        var distanceSquared = DistanceSquared(a, b);
        return MathF.Sqrt(distanceSquared);
    }

    public static int DistanceSquared(Vector2Int a, Vector2Int b)
    {
        var difference = a - b;
        return Dot(difference, difference);
    }

    public static int Dot(Vector2Int a, Vector2Int b) => a.X * b.X + a.Y * b.Y;


    public static Vector2Int Abs(Vector2Int value) => new Vector2Int(Math.Abs(value.X), Math.Abs(value.Y));
}