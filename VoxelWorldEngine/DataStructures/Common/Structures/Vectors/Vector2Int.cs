namespace VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

public partial struct Vector2Int
{
    public int X, Y;

    public const int Count = 2;

    // constructors
    public Vector2Int(int value)
    {
        X = value;
        Y = value;
    }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector2Int Zero => new(0, 0);
    public static Vector2Int One => new(1, 1);
    public static Vector2Int UnitX => new(1, 0);
    public static Vector2Int UnitY => new(0, 1);


    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"{X}, {Y}";
    }
}