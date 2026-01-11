namespace VoxelWorldEngine.DataStructures.Vector2Int;

public partial struct Vector2Int : IEquatable<Vector2Int>
{
    // equality 
    public static bool operator ==(Vector2Int left, Vector2Int right) => left.X == right.X && left.Y == right.Y;
    public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);
    public bool Equals(Vector2Int other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Vector2Int other && Equals(other);


    // compare 

    public static bool operator >(Vector2Int left, Vector2Int right) => left.LengthSquared() > right.LengthSquared();
    public static bool operator <(Vector2Int left, Vector2Int right) => left.LengthSquared() < right.LengthSquared();

    // add
    public static Vector2Int operator +(Vector2Int left, Vector2Int right) => new(left.X + right.X, left.Y + right.Y);

    // subtract
    public static Vector2Int operator -(Vector2Int left, Vector2Int right) => new(left.X - right.X, left.Y - right.Y);
    public static Vector2Int operator -(Vector2Int value) => Zero - value;

    // multiply
    public static Vector2Int operator *(Vector2Int left, Vector2Int right) => new(left.X * right.X, left.Y * right.Y);
    public static Vector2Int operator *(Vector2Int left, int right) => left * new Vector2Int(right);
    public static Vector2Int operator *(int left, Vector2Int right) => right * left;

    // divide
    public static Vector2Int operator /(Vector2Int left, Vector2Int right) => new(left.X / right.X, left.Y / right.Y);
    public static Vector2Int operator /(Vector2Int left, int right) => left / new Vector2Int(right);
}