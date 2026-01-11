namespace VoxelWorldEngine.DataStructures.Vector3Int;

public partial struct Vector3Int : IEquatable<Vector3Int>
{
    // equality 
    public static bool operator ==(Vector3Int left, Vector3Int right)
        => left.X == right.X && left.Y == right.Y && left.Z == right.Z;

    public static bool operator !=(Vector3Int left, Vector3Int right)
        => !(left == right);

    public bool Equals(Vector3Int other)
        => X == other.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object? obj)
        => obj is Vector3Int other && Equals(other);

    // compare 
    public static bool operator >(Vector3Int left, Vector3Int right)
        => left.LengthSquared() > right.LengthSquared();

    public static bool operator <(Vector3Int left, Vector3Int right)
        => left.LengthSquared() < right.LengthSquared();

    // add
    public static Vector3Int operator +(Vector3Int left, Vector3Int right)
        => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    // subtract
    public static Vector3Int operator -(Vector3Int left, Vector3Int right)
        => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector3Int operator -(Vector3Int value)
        => Zero - value;

    // multiply
    public static Vector3Int operator *(Vector3Int left, Vector3Int right)
        => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Vector3Int operator *(Vector3Int left, int right)
        => left * new Vector3Int(right);

    public static Vector3Int operator *(int left, Vector3Int right)
        => right * left;

    // divide
    public static Vector3Int operator /(Vector3Int left, Vector3Int right)
        => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Vector3Int operator /(Vector3Int left, int right)
        => left / new Vector3Int(right);
}