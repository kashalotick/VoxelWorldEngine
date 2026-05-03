using GameApp.Content.Systems;
using GameEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class FlightMovementStrategy : BaseMovementStrategy
{
    private readonly float _verticalSpeed = 1f;

    public FlightMovementStrategy(
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
    
    public override void Update(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        ApplyMouseLook(mouse);

        var movement = GetFlatMoveDirection(keyboard);
        var vertical = 0f;
        if (keyboard.IsKeyDown(Keys.Space)) vertical += 1;
        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)) vertical -= 1;

        var velocity = movement * CalculateSpeed(MoveSpeed, keyboard) + Vector3.UnitY * vertical * CalculateSpeed(_verticalSpeed, keyboard);

        ApplyPhysics(deltaTime, velocity);
    }

    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Physics.SetVelocity(velocity);
        Physics.Update((float)deltaTime, false, true);
        Camera.Position = Physics.Position;
    }
}