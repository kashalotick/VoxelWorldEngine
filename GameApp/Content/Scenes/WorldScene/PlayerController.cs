using GameApp.Debug;
using LearningOpenTK.Content.Input;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Scenes.WorldScene;

public class PlayerController : FreeCameraController
{
    public event Action Pause;
    public event Action ToggleHud;
    
    public event Action PlaceBlock;
    public event Action BreakBlock;

    public event Action InventoryNext;
    public event Action InventoryPrevious;


    private RayShooter _rayShooter;

    public PlayerController(Camera camera, RayShooter rayShooter) : base(camera)
    {
        _rayShooter = rayShooter;
    }

    public override void OnKeyDown(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        if (e.Key == Keys.Escape) Pause?.Invoke();
    }

    public override void OnKeyUp(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        switch (e.Key)
        {
            case Keys.F3:
                ToggleHud?.Invoke();
                break;
            case Keys.F11:
                RequestWindowAction(new ToggleFullscreenWindow()); // TODO: replace with event???
                break;
        }
    }

    private enum ClickState { Idle, WaitingForFirstRepeat, Repeating }
    private ClickState _currentClickState = ClickState.Idle;
    
    private double _timer = 0;    private const double InitialDelay = 0.25;
    private const double RepeatInterval = 0.025; 
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
            _rayShooter.Trace();
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
            // _rayShooter.Trace();
            BreakBlock?.Invoke();
        }
        if (e.Button == MouseButton.Right)
        {
            PlaceBlock?.Invoke();
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