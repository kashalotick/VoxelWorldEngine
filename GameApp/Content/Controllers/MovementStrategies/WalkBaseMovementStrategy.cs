using GameApp.Content.Systems;
using LearningOpenTK.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class WalkBaseMovementStrategy : BaseMovementStrategy
{
    public float JumpForce { get; }
    private bool _jumpHeld;

    public WalkBaseMovementStrategy(Camera camera, CharacterPhysics physics, float mouseSensitivity, float moveSpeed, float sprintMultiplier, float jumpForce)
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier)
    {
        JumpForce = jumpForce;
    }

    public override MovementMode Mode => MovementMode.Walk;

    public override void Update(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        ApplyMouseLook(mouse);

        var movement = GetFlatMoveDirection(keyboard);
        var speed = CalculateSpeed(keyboard);

        var velocity = new Vector2(movement.X, movement.Z) * speed;
        Physics.SetHorizontalVelocity(velocity);

        var jumpDown = keyboard.IsKeyDown(Keys.Space);
        if (jumpDown && !_jumpHeld)
        {
            Physics.Jump(JumpForce);
        }

        _jumpHeld = jumpDown;

        ApplyPhysics(deltaTime, new Vector3(velocity));
    }

    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Physics.Update((float)deltaTime, applyGravity: true, resolveCollisions: true);
        Camera.Position = Physics.Position;
    }
}