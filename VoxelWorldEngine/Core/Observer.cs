using System.Diagnostics.CodeAnalysis;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Core;

public struct Observer : IEquatable<Observer>
{
    public Vector3Int Position;
    public int ViewRadius;

    public Observer(Vector3Int position, int viewRadius)
    {
        Position = position;
        ViewRadius = viewRadius;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Observer other)
    {
        return Position.Equals(other.Position) && ViewRadius == other.ViewRadius;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, ViewRadius);
    }

    public static bool operator ==(Observer left, Observer right)
        => left.Position == right.Position && left.ViewRadius == right.ViewRadius;

    public static bool operator !=(Observer left, Observer right) => !(left == right);
}