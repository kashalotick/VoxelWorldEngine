namespace VoxelWorldEngine.Utils.Vector3Int;

public partial struct Vector3Int : IEquatable<Vector3Int>
{

    
    // equality 
    public static bool operator ==(Vector3Int left, Vector3Int right)
    {
        return (left.X == right.X)
               && (left.Y == right.Y)
               && (left.Z == right.Z);
    }

    public static bool operator !=(Vector3Int left, Vector3Int right)
    {
        return !(left == right);
    }
    public bool Equals(Vector3Int other)
    {
        return X == other.X
               && Y == other.Y
               && Z == other.Z;
    }
    public override bool Equals(object? obj)
    {
        return obj is Vector3Int other && Equals(other);
    }
    // add
    public static Vector3Int operator +(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    // subtract
    public static Vector3Int operator -(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static Vector3Int operator -(Vector3Int value)
    {
        return Zero - value;
    }

    // multiply
    public static Vector3Int operator *(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector3Int operator *(Vector3Int left, int right)
    {
        return left * new Vector3Int(right);
    }

    public static Vector3Int operator *(int left, Vector3Int right)
    {
        return right * left;
    }
    // divide

    public static Vector3Int operator /(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector3Int operator /(Vector3Int left, int right)
    {
        return left / new Vector3Int(right);
    }
}