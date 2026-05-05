using System.Numerics;

namespace VoxelModule.Core.Vectors;

public partial struct Vector3Int : IEquatable<Core.Vectors.Vector3Int>
{
    // equality 
    public static bool operator ==(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
    }

    public static bool operator !=(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return !(left == right);
    }

    public bool Equals(Core.Vectors.Vector3Int other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is Core.Vectors.Vector3Int other && Equals(other);
    }

    // compare 
    public static bool operator >(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return left.LengthSquared() > right.LengthSquared();
    }

    public static bool operator <(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return left.LengthSquared() < right.LengthSquared();
    }

    // add
    public static Core.Vectors.Vector3Int operator +(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return new Core.Vectors.Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    // subtract
    public static Core.Vectors.Vector3Int operator -(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return new Core.Vectors.Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static Core.Vectors.Vector3Int operator -(Core.Vectors.Vector3Int value)
    {
        return Zero - value;
    }

    // multiply
    public static Core.Vectors.Vector3Int operator *(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return new Core.Vectors.Vector3Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    public static Core.Vectors.Vector3Int operator *(Core.Vectors.Vector3Int left, int right)
    {
        return left * new Core.Vectors.Vector3Int(right);
    }

    public static Core.Vectors.Vector3Int operator *(int left, Core.Vectors.Vector3Int right)
    {
        return right * left;
    }

    // divide
    public static Core.Vectors.Vector3Int operator /(Core.Vectors.Vector3Int left, Core.Vectors.Vector3Int right)
    {
        return new Core.Vectors.Vector3Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }

    public static Core.Vectors.Vector3Int operator /(Core.Vectors.Vector3Int left, int right)
    {
        return left / new Core.Vectors.Vector3Int(right);
    }
    
    // type cast

    // public static implicit operator Vector3(Vector3Int vector)
    //     => new Vector3(vector.X, vector.Y, vector.Z);

    public static explicit operator Vector3(Core.Vectors.Vector3Int vector) 
        => new Vector3(vector.X, vector.Y, vector.Z);
}