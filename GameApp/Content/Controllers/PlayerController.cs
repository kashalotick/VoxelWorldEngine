using GameApp.Content.Controllers.MovementStrategies;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameEngine.Core;
using GameEngine.Engine.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers;

public class PlayerController : SceneController
{
    private const double DoubleTapWindow = 0.35;
    private const double InitialDelay = 0.25;
    private const double RepeatInterval = 0.2;
    private double _timer;
    private ClickState _currentClickState;

    private readonly FlightMovementStrategy _flightStrategy;
    private readonly FreeCameraMovementStrategy _freeCameraStrategy;
    private readonly WalkMovementStrategy _walkStrategy;
    private readonly Dictionary<IMovementStrategy, MovementMode> _movementModeMap;
    private IMovementStrategy _movementStrategy;
    

    public PlayerController(Camera camera, CharacterPhysicsService physics)
    {
        _walkStrategy
            = new WalkMovementStrategy(
                camera,
                physics,
                MouseSensitivity,
                MoveSpeed,
                SprintMultiplier,
                JumpForce);
        _flightStrategy = new FlightMovementStrategy(
            camera,
            physics,
            MouseSensitivity,
            FlightSpeed,
            FlightSprintMultiplier);
        _freeCameraStrategy = new FreeCameraMovementStrategy(
            camera,
            physics,
            MouseSensitivity,
            FlightSpeed,
            FlightSprintMultiplier);
        _movementModeMap = new Dictionary<IMovementStrategy, MovementMode>
        {
            { _walkStrategy, MovementMode.Walk },
            { _flightStrategy, MovementMode.Flight },
            { _freeCameraStrategy, MovementMode.FreeCamera }
        };
        _movementStrategy = _walkStrategy;
    }

    public float MouseSensitivity { get; set; } = 0.2f;
    public float MoveSpeed { get; set; } = 5f;
    public float SprintMultiplier { get; set; } = 1.75f;
    public float FlightSpeed { get; set; } = 10f;
    public float FlightSprintMultiplier { get; set; } = 2.5f;
    public float JumpForce { get; set; } = 8f;

    
    public event Action? Pause;
    public event Action? ToggleHud;
    public event Action? ToggleDebug;

    public event Action? PlaceBlock;
    public event Action? BreakBlock;
    public event Action? MiddleButtonClick;

    public event Action? InventoryNext;
    public event Action? InventoryPrevious;

    public event Action<MovementMode>? SetMovementMode;
    public event Action<int>? SetBrushSize;
    public event Action? ToggleBrushType;

    public override void OnUpdate(double deltaTime, KeyboardState keyboard, MouseState mouse)
    {
        base.OnUpdate(deltaTime, keyboard, mouse);

        _movementStrategy.Update(deltaTime, keyboard, mouse);
    }

    public override void OnKeyDown(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        if (e.IsRepeat) return;

        if (e.Key == Keys.Escape)
        {
            Pause?.Invoke();
            return;
        }

        if (e.Key == Keys.Tab)
        {
            ToggleFlightMode();
            return;
        }

        if (e.Key == Keys.F)
        {
            ToggleFreeCam();
            return;
        }

        if (e.Key == Keys.B)
        {
            ToggleBrushType?.Invoke();
            return;
        }

        var brushSize = e.Key switch
        {
            Keys.D1 => 1,
            Keys.D2 => 2,
            Keys.D3 => 3,
            Keys.D4 => 4,
            Keys.D5 => 5,
            Keys.D6 => 6,
            Keys.D7 => 7,
            Keys.D8 => 8,
            Keys.D9 => 9,
            _ => 0
        };
        if (brushSize != 0)
        {
            SetBrushSize?.Invoke(brushSize);
        }
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

    private void ToggleFreeCam()
    {
        if (_movementStrategy == _freeCameraStrategy) SetMovementStrategy(_walkStrategy);
        else SetMovementStrategy(_freeCameraStrategy);
    }

    private void ToggleFlightMode()
    {
        if (_movementStrategy == _flightStrategy) SetMovementStrategy(_walkStrategy);
        else SetMovementStrategy(_flightStrategy);
    }

    private void SetMovementStrategy(IMovementStrategy strategy)
    {
        if (_movementStrategy == strategy) return;

        _movementStrategy = strategy;
        SetMovementMode?.Invoke(_movementModeMap[_movementStrategy]);
    }

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

    private enum ClickState
    {
        Idle,
        WaitingForFirstRepeat,
        Repeating
    }
}
