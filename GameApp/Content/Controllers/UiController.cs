using GameApp.Application;
using GameEngine.Core;
using GameEngine.Engine;
using GameEngine.Engine.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers;

// TODO: make ui controller interface
public class UiController : SceneController
{
    public UiController(MyGameContext gameContext)
    {
        GameContext = gameContext;
    }


    public MyGameContext GameContext { get; }
    public event Action<Vector2> Click;
    public event Action<Vector2> MouseMove;
    public event Action<KeyboardKeyEventArgs> KeyUp;
    public event Action<KeyboardKeyEventArgs> KeyDown;
    public event Action<TextInputEventArgs> TextInput;

    public override void OnKeyUp(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        switch (e.Key)
        {
            case Keys.F11:
                RequestWindowAction(new ToggleFullscreenWindow());
                break;
        }

        KeyUp?.Invoke(e);
    }

    public override void OnKeyDown(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        KeyDown?.Invoke(e);
    }

    public override void OnTextInput(TextInputEventArgs e, KeyboardState keyboard)
    {
        TextInput?.Invoke(e);
    }

    public override void OnMouseUp(MouseButtonEventArgs e, MouseState mouse)
    {
        var pos = new Vector2(mouse.Position.X, GameContext.ScreenHeight - mouse.Position.Y);
        Click?.Invoke(pos);
    }

    public override void OnMouseMove(MouseMoveEventArgs e, MouseState mouse)
    {
        var pos = new Vector2(mouse.Position.X, GameContext.ScreenHeight - mouse.Position.Y);
        MouseMove?.Invoke(pos);
    }
}