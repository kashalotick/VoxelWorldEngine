using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public class WalkMovementStrategy : BaseMovementStrategy
{
    private bool _jumpHeld;
    private float JumpForce { get; }
    private double _jumpBufferTimer = 0;
    private const double _jumpBufferTime = 0.1;

    public WalkMovementStrategy(
        Camera camera,
        CharacterPhysicsService physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier,
        float jumpForce
    )
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier)
    {
        JumpForce = jumpForce;
    }
    
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

        Physics.Update((float)deltaTime, true, true);
        Camera.Position = Physics.Position;
    }
}