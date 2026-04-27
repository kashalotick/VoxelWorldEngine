using GameApp.Content.Systems;
using LearningOpenTK.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class FlightBaseMovementStrategy : BaseMovementStrategy
{
    private readonly float _verticalSpeed = 1f;

    public FlightBaseMovementStrategy(
        Camera camera,
        CharacterPhysics physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier,
        float verticalSpeed
    )
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier)
    {
        _verticalSpeed = verticalSpeed;
    }

    public override MovementMode Mode => MovementMode.Flight;

    public override void Update(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        ApplyMouseLook(mouse);

        var movement = GetFlatMoveDirection(keyboard);
        var speed = CalculateSpeed(keyboard);

        var vertical = 0f;
        if (keyboard.IsKeyDown(Keys.Space)) vertical += 1;
        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)) vertical -= 1;

        var velocity = movement * speed + Vector3.UnitY * vertical * _verticalSpeed;

        ApplyPhysics(deltaTime, velocity);
    }

    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Physics.SetVelocity(velocity);
        Physics.Update((float)deltaTime, false, true);
        Camera.Position = Physics.Position;
    }
}