using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.scenes;


// TODO: make ui controller interface
public class UiController : SceneController
{
    public event Action<Vector2> Click;
    public event Action<Vector2> MouseMove;
    public event Action<KeyboardKeyEventArgs> KeyDown;
    public event Action<TextInputEventArgs> TextInput;



    public GameContext GameContext { get; }

    public UiController(GameContext gameContext)
    {
        GameContext = gameContext;
    }

    public override void OnKeyUp(KeyboardKeyEventArgs e, KeyboardState keyboard)
    {
        switch (e.Key)
        {
            case Keys.F11:
                RequestWindowAction(new ToggleFullscreenWindow());
                break;
        }
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