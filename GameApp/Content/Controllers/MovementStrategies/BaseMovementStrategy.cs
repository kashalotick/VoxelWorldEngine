using GameApp.Content.Systems;
using GameEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public abstract class BaseMovementStrategy : IMovementStrategy
{
    protected readonly Camera Camera;
    protected readonly CharacterPhysics Physics;

    private float MouseSensitivity { get; }
    protected float MoveSpeed { get; }
    private float SprintMultiplier { get; }
    
    protected BaseMovementStrategy(
        Camera camera,
        CharacterPhysics physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier
    )
    {
        Camera = camera;
        Physics = physics;
        MouseSensitivity = mouseSensitivity;
        MoveSpeed = moveSpeed;
        SprintMultiplier = sprintMultiplier;
    }
    
    public abstract void Update(double deltaTime, KeyboardState keyboard, MouseState mouse);

    protected void ApplyMouseLook(MouseState mouse)
    {
        Camera.Yaw += mouse.Delta.X * MouseSensitivity;
        Camera.Pitch = MathHelper.Clamp(Camera.Pitch - mouse.Delta.Y * MouseSensitivity, -89f, 89f);
    }

    protected Vector3 GetFlatForward()
    {
        var yawRad = MathHelper.DegreesToRadians(Camera.Yaw);
        return Vector3.Normalize(new Vector3(MathF.Cos(yawRad), 0f, MathF.Sin(yawRad)));
    }

    protected Vector3 GetFlatRight()
    {
        var forward = GetFlatForward();
        return Vector3.Normalize(new Vector3(-forward.Z, 0f, forward.X));
    }

    protected Vector3 GetFlatMoveDirection(KeyboardState keyboard)
    {
        var forward = GetFlatForward();
        var right = GetFlatRight();
        var movement = Vector3.Zero;

        if (keyboard.IsKeyDown(Keys.W)) movement += forward;
        if (keyboard.IsKeyDown(Keys.S)) movement -= forward;
        if (keyboard.IsKeyDown(Keys.D)) movement += right;
        if (keyboard.IsKeyDown(Keys.A)) movement -= right;

        return movement.LengthSquared > 0f ? Vector3.Normalize(movement) : Vector3.Zero;
    }

    protected float CalculateSpeed(float moveSpeed, KeyboardState keyboard)
    {
        var speed = keyboard.IsKeyDown(Keys.LeftControl) ? moveSpeed * SprintMultiplier : moveSpeed;
        return speed;
    }
}