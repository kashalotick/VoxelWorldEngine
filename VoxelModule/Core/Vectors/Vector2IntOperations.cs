using System.Numerics;

namespace VoxelModule.Core.Vectors;

public partial struct Vector2Int : IEquatable<Core.Vectors.Vector2Int>
{
    // equality 
    public static bool operator ==(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return !(left == right);
    }

    public bool Equals(Core.Vectors.Vector2Int other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Core.Vectors.Vector2Int other && Equals(other);
    }


    // compare 

    public static bool operator >(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return left.LengthSquared() > right.LengthSquared();
    }

    public static bool operator <(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return left.LengthSquared() < right.LengthSquared();
    }

    // add
    public static Core.Vectors.Vector2Int operator +(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return new Core.Vectors.Vector2Int(left.X + right.X, left.Y + right.Y);
    }

    // subtract
    public static Core.Vectors.Vector2Int operator -(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return new Core.Vectors.Vector2Int(left.X - right.X, left.Y - right.Y);
    }

    public static Core.Vectors.Vector2Int operator -(Core.Vectors.Vector2Int value)
    {
        return Zero - value;
    }

    // multiply
    public static Core.Vectors.Vector2Int operator *(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return new Core.Vectors.Vector2Int(left.X * right.X, left.Y * right.Y);
    }

    public static Core.Vectors.Vector2Int operator *(Core.Vectors.Vector2Int left, int right)
    {
        return left * new Core.Vectors.Vector2Int(right);
    }

    public static Core.Vectors.Vector2Int operator *(int left, Core.Vectors.Vector2Int right)
    {
        return right * left;
    }

    // divide
    public static Core.Vectors.Vector2Int operator /(Core.Vectors.Vector2Int left, Core.Vectors.Vector2Int right)
    {
        return new Core.Vectors.Vector2Int(left.X / right.X, left.Y / right.Y);
    }

    public static Core.Vectors.Vector2Int operator /(Core.Vectors.Vector2Int left, int right)
    {
        return left / new Core.Vectors.Vector2Int(right);
    }
    
    // type cast
    
    public static explicit operator Vector2(Core.Vectors.Vector2Int vector) 
        => new Vector2(vector.X, vector.Y);
}