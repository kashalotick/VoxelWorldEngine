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
                RequestWindowAction(new ToggleFullscreenWindow()); // TODO: replace with event
                break;
        }
    }

    public override void OnMouseDown(MouseButtonEventArgs e, MouseState mouse)
    {
        if (e.Button == MouseButton.Left)
        {
            _rayShooter.Trace();
        }
    }
}