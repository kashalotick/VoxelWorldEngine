using OpenTK.Mathematics;
using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace GameApp.Content.Systems;

public class CharacterPhysics
{
    private const float DefaultGravity = -25f;
    private const float DefaultTerminalVelocity = -50f;
    private const float StepSize = 0.05f;
    private readonly Func<Vector3Int, bool> _isSolidVoxel;

    public CharacterPhysics(Vector3 initialPosition, Func<Vector3Int, bool> isSolidVoxel)
    {
        Position = initialPosition;
        _isSolidVoxel = isSolidVoxel;
    }

    public float Gravity { get; set; } = DefaultGravity;
    public float TerminalVelocity { get; set; } = DefaultTerminalVelocity;

    public Vector3 Position { get; private set; }
    public Vector3 Velocity { get; private set; }
    public bool IsGrounded { get; private set; }

    public float Width { get; set; } = 0.6f;
    public float Height { get; set; } = 1.8f;
    public float EyeHeight { get; set; } = 1.65f;

    public void Teleport(Vector3 position)
    {
        Position = position;
        Velocity = Vector3.Zero;
        IsGrounded = false;
    }

    public void SetVelocity(Vector3 velocity)
    {
        Velocity = velocity;
    }

    public void SetHorizontalVelocity(Vector2 velocityXz)
    {
        Velocity = new Vector3(velocityXz.X, Velocity.Y, velocityXz.Y);
    }

    public void Jump(float jumpSpeed)
    {
        if (!IsGrounded) return;

        Velocity = new Vector3(Velocity.X, jumpSpeed, Velocity.Z);
        IsGrounded = false;
    }

    public void Update(float deltaTime)
    {
        Update(deltaTime, true, true);
    }

    public void Update(float deltaTime, bool applyGravity, bool resolveCollisions)
    {
        if (deltaTime <= 0f) return;

        if (applyGravity)
        {
            var verticalVelocity = Velocity.Y + Gravity * deltaTime;
            verticalVelocity = MathF.Max(verticalVelocity, TerminalVelocity);
            Velocity = new Vector3(Velocity.X, verticalVelocity, Velocity.Z);
        }

        IsGrounded = false;

        if (!resolveCollisions)
        {
            Position += Velocity * deltaTime;
            return;
        }

        var movement = Velocity * deltaTime;
        var nextPosition = Position;

        MoveAxis(ref nextPosition, movement.X, 0);
        MoveAxis(ref nextPosition, movement.Y, 1);
        MoveAxis(ref nextPosition, movement.Z, 2);

        Position = nextPosition;
    }

    private void MoveAxis(ref Vector3 position, float delta, int axis)
    {
        if (MathF.Abs(delta) <= float.Epsilon) return;

        var steps = Math.Max(1, (int)MathF.Ceiling(MathF.Abs(delta) / StepSize));
        var stepDelta = delta / steps;

        for (var i = 0; i < steps; i++)
        {
            var trial = position;
            SetAxis(ref trial, axis, GetAxis(trial, axis) + stepDelta);

            if (IntersectsSolid(trial, _isSolidVoxel))
            {
                ZeroVelocityAxis(axis);
                if (axis == 1 && delta < 0f) IsGrounded = true;
                return;
            }

            position = trial;
        }
    }
    

    public bool IntersectsSolid(Vector3 cameraPosition, Func<Vector3Int, bool> isSolidVoxel)
    {
        var halfWidth = Width * 0.5f;

        var min = new Vector3(
            cameraPosition.X - halfWidth,
            cameraPosition.Y - EyeHeight,
            cameraPosition.Z - halfWidth
        );
        var max = new Vector3(
            cameraPosition.X + halfWidth,
            cameraPosition.Y + (Height - EyeHeight),
            cameraPosition.Z + halfWidth
        );

        const float epsilon = 0.0001f;
        var minX = (int)MathF.Floor(min.X);
        var minY = (int)MathF.Floor(min.Y);
        var minZ = (int)MathF.Floor(min.Z);
        var maxX = (int)MathF.Floor(max.X - epsilon);
        var maxY = (int)MathF.Floor(max.Y - epsilon);
        var maxZ = (int)MathF.Floor(max.Z - epsilon);

        for (var x = minX; x <= maxX; x++)
        for (var y = minY; y <= maxY; y++)
        for (var z = minZ; z <= maxZ; z++)
        {
            if (isSolidVoxel(new Vector3Int(x, y, z)))
            {
                return true;
            }
        }

        return false;
    }

    private void ZeroVelocityAxis(int axis)
    {
        Velocity = axis switch
        {
            0 => new Vector3(0f, Velocity.Y, Velocity.Z),
            1 => new Vector3(Velocity.X, 0f, Velocity.Z),
            2 => new Vector3(Velocity.X, Velocity.Y, 0f),
            _ => Velocity
        };
    }

    private static float GetAxis(Vector3 value, int axis)
    {
        return axis switch
        {
            0 => value.X,
            1 => value.Y,
            2 => value.Z,
            _ => 0f
        };
    }

    private static void SetAxis(ref Vector3 value, int axis, float axisValue)
    {
        switch (axis)
        {
            case 0:
                value.X = axisValue;
                break;
            case 1:
                value.Y = axisValue;
                break;
            case 2:
                value.Z = axisValue;
                break;
        }
    }
}