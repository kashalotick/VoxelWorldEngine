using GameApp.Content.Systems;
using LearningOpenTK.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class WalkBaseMovementStrategy : BaseMovementStrategy
{
    private bool _jumpHeld;

    public WalkBaseMovementStrategy(
        Camera camera,
        CharacterPhysics physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier,
        float jumpForce
    )
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier)
    {
        JumpForce = jumpForce;
    }

    public float JumpForce { get; }

    public override MovementMode Mode => MovementMode.Walk;


    private double _jumpBufferTimer = 0;
    private const double _jumpBufferTime = 0.1;

    public override void Update(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        ApplyMouseLook(mouse);

        var movement = GetFlatMoveDirection(keyboard);
        var speed = CalculateSpeed(MoveSpeed, keyboard);
        var velocity = new Vector2(movement.X, movement.Z) * speed;
        Physics.SetHorizontalVelocity(velocity);

        if (_jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= deltaTime;
        }
        
        bool jumpDown = keyboard.IsKeyDown(Keys.Space);
        if (jumpDown && !_jumpHeld) 
        {
            _jumpBufferTimer = _jumpBufferTime;
        }
        _jumpHeld = jumpDown;
        
        if (_jumpBufferTimer > 0 && Physics.IsGrounded) 
        {
            Physics.Jump(JumpForce);
            _jumpBufferTimer = 0;
        }

        ApplyPhysics(deltaTime, new Vector3(velocity));
    }

    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Physics.Update((float)deltaTime, true, true);
        Camera.Position = Physics.Position;
    }
}