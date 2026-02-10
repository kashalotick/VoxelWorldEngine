namespace VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

public partial struct Vector2Int : IEquatable<Vector2Int>
{
    // equality 
    public static bool operator ==(Vector2Int left, Vector2Int right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Vector2Int left, Vector2Int right)
    {
        return !(left == right);
    }

    public bool Equals(Vector2Int other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && Equals(other);
    }


    // compare 

    public static bool operator >(Vector2Int left, Vector2Int right)
    {
        return left.LengthSquared() > right.LengthSquared();
    }

    public static bool operator <(Vector2Int left, Vector2Int right)
    {
        return left.LengthSquared() < right.LengthSquared();
    }

    // add
    public static Vector2Int operator +(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X + right.X, left.Y + right.Y);
    }

    // subtract
    public static Vector2Int operator -(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X - right.X, left.Y - right.Y);
    }

    public static Vector2Int operator -(Vector2Int value)
    {
        return Zero - value;
    }

    // multiply
    public static Vector2Int operator *(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X * right.X, left.Y * right.Y);
    }

    public static Vector2Int operator *(Vector2Int left, int right)
    {
        return left * new Vector2Int(right);
    }

    public static Vector2Int operator *(int left, Vector2Int right)
    {
        return right * left;
    }

    // divide
    public static Vector2Int operator /(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X / right.X, left.Y / right.Y);
    }

    public static Vector2Int operator /(Vector2Int left, int right)
    {
        return left / new Vector2Int(right);
    }
}