namespace VoxelWorldEngine.DataStructures.Vector3Int;

public partial struct Vector3Int
{
    public int X, Y, Z;

    public const int Count = 3;

    // constructors
    public Vector3Int(int value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    public Vector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3Int Zero => new(0, 0, 0);
    public static Vector3Int One => new(1, 1, 1);
    public static Vector3Int UnitX => new(1, 0, 0);
    public static Vector3Int UnitY => new(0, 1, 0);
    public static Vector3Int UnitZ => new(0, 0, 1);



    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }
}