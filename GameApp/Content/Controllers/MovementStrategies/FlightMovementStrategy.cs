using GameApp.Content.Systems;
using GameEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class FlightMovementStrategy : BaseMovementStrategy
{
    public FlightMovementStrategy(
        Camera camera,
        CharacterPhysics physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier
    )
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier) { }

    public override void Update(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        ApplyMouseLook(mouse);

        var movement = GetFlatMoveDirection(keyboard);
        var vertical = 0f;
        if (keyboard.IsKeyDown(Keys.Space)) vertical += 1;
        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
            vertical -= 1;

        var velocity = movement * CalculateSpeed(MoveSpeed, keyboard)
                       + Vector3.UnitY * vertical * CalculateSpeed(MoveSpeed, keyboard);

        Physics.SetVelocity(velocity);
        Physics.Update((float)deltaTime, false, true);
        Camera.Position = Physics.Position;
    }
}
