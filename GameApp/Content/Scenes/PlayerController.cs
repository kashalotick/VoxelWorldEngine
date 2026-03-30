using GameApp.Debug;
using LearningOpenTK.Content.Input;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Scenes;

public class PlayerController : FreeCameraController
{
    private RayShooter _rayShooter;

    public PlayerController(Camera camera, RayShooter rayShooter) : base(camera)
    {
        _rayShooter = rayShooter;
    }


    public override void OnKeyUp(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        base.OnKeyUp(e, keyboard);
        switch (e.Key)
        {
            case Keys.F11:
                RequestWindowAction(new ToggleFullscreenWindow());
                break;
            case Keys.Escape:
                RequestWindowAction(new CloseWindow());
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