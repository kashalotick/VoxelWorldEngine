using GameApp.Debug;
using GameApp.Content.Systems;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Scenes.WorldScene;

public class PlayerController : SceneController
{
    public event Action Pause;
    public event Action ToggleHud;
    public event Action ToggleDebug;

    public event Action PlaceBlock;
    public event Action BreakBlock;
    public event Action MiddleButtonClick;

    public event Action InventoryNext;
    public event Action InventoryPrevious;


    private Raycaster _raycaster;
    private readonly Camera _camera;
    private readonly CharacterPhysics _physics;

    private bool _jumpHeld;

    public float MouseSensitivity { get; set; } = 0.2f;
    public float MoveSpeed { get; set; } = 5f;
    public float SprintMultiplier { get; set; } = 2f;
    public float JumpForce { get; set; } = 8f;

    public PlayerController(Camera camera, Raycaster raycaster, CharacterPhysics physics)
    {
        _camera = camera;
        _raycaster = raycaster;
        _physics = physics;
    }

    public override void OnUpdate(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        base.OnUpdate(deltaTime, keyboard, mouse);

        ProcessMouseLook(mouse);
        ProcessMovementInput(keyboard);
        ProcessJumpInput(keyboard);

        _physics.Update((float)deltaTime);
        _camera.Position = _physics.Position;
    }

    public override void OnKeyDown(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        if (e.Key == Keys.Escape) Pause?.Invoke();
    }

    public override void OnKeyUp(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        switch (e.Key)
        {
            case Keys.F1:
                ToggleHud?.Invoke();
                break;
            case Keys.F3:
                ToggleDebug?.Invoke();
                break;
            case Keys.F11:
                RequestWindowAction(new ToggleFullscreenWindow()); // TODO: replace with event???
                break;
        }
    }

    private void ProcessMouseLook(MouseState mouse)
    {
        _camera.Yaw += mouse.Delta.X * MouseSensitivity;
        _camera.Pitch = MathHelper.Clamp(_camera.Pitch - mouse.Delta.Y * MouseSensitivity, -89f, 89f);
    }

    private void ProcessMovementInput(KeyboardState keyboard)
    {
        var yawRad = MathHelper.DegreesToRadians(_camera.Yaw);
        var forward = Vector3.Normalize(new Vector3(MathF.Cos(yawRad), 0f, MathF.Sin(yawRad)));
        var right = Vector3.Normalize(new Vector3(-forward.Z, 0f, forward.X));

        var direction = Vector3.Zero;

        if (keyboard.IsKeyDown(Keys.W)) direction += forward;
        if (keyboard.IsKeyDown(Keys.S)) direction -= forward;
        if (keyboard.IsKeyDown(Keys.D)) direction += right;
        if (keyboard.IsKeyDown(Keys.A)) direction -= right;

        if (direction.LengthSquared > 0f)
        {
            direction = Vector3.Normalize(direction);
        }

        var speed = keyboard.IsKeyDown(Keys.LeftControl)
            ? MoveSpeed * SprintMultiplier
            : MoveSpeed;

        var horizontalVelocity = new Vector2(direction.X * speed, direction.Z * speed);
        _physics.SetHorizontalVelocity(horizontalVelocity);
    }

    private void ProcessJumpInput(KeyboardState keyboard)
    {
        var jumpDown = keyboard.IsKeyDown(Keys.Space);
        if (jumpDown && !_jumpHeld && _physics.IsGrounded)
        {
            _physics.Jump(JumpForce);
        }

        _jumpHeld = jumpDown;
    }

    private enum ClickState
    {
        Idle,
        WaitingForFirstRepeat,
        Repeating
    }

    private ClickState _currentClickState = ClickState.Idle;

    private double _timer = 0;
    private const double InitialDelay = 0.25;
    private const double RepeatInterval = 0.2;

    public override void OnMousePressed(double deltaTime, MouseState mouse)
    {
        if (mouse.IsAnyButtonDown)
        {
            switch (_currentClickState)
            {
                case ClickState.Idle:
                    // DoAction(); 

                    _timer = InitialDelay;
                    _currentClickState = ClickState.WaitingForFirstRepeat;
                    break;

                case ClickState.WaitingForFirstRepeat:
                    _timer -= deltaTime;
                    if (_timer <= 0)
                    {
                        // DoAction();

                        _timer = RepeatInterval;
                        _currentClickState = ClickState.Repeating;
                    }

                    break;

                case ClickState.Repeating:
                    _timer -= deltaTime;
                    if (_timer <= 0)
                    {
                        OnMouseButtonPressed(mouse);
                        // DoAction();
                        _timer = RepeatInterval;
                    }

                    break;
            }
        }
        else
        {
            _currentClickState = ClickState.Idle;
            _timer = 0;
        }
    }

    private void OnMouseButtonPressed(MouseState mouse)
    {
        if (mouse.IsButtonDown(MouseButton.Left))
        {
            _raycaster.Trace();
            BreakBlock?.Invoke();
        }

        if (mouse.IsButtonDown(MouseButton.Right))
        {
            PlaceBlock?.Invoke();
        }
    }

    public override void OnMouseDown(MouseButtonEventArgs e, MouseState mouse)
    {
        if (e.Button == MouseButton.Left)
        {
            _raycaster.Trace();
            BreakBlock?.Invoke();
        }

        if (e.Button == MouseButton.Right)
        {
            PlaceBlock?.Invoke();
        }

        if (e.Button == MouseButton.Middle)
        {
            MiddleButtonClick?.Invoke();
        }
    }

    public override void OnMouseWheel(MouseWheelEventArgs e, MouseState mouse)
    {
        if (e.OffsetY > 0)
        {
            InventoryPrevious?.Invoke();
        }
        else if (e.OffsetY < 0)
        {
            InventoryNext?.Invoke();
        }
    }
}