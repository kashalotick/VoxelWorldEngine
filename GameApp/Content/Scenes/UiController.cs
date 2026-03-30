using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.scenes;

public class UiController : SceneController
{
    public event Action<Vector2> Click;
    public event Action<Vector2> MouseMove;

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

    public override void OnTextInput(TextInputEventArgs e, KeyboardState keyboard)
    {
        Console.WriteLine($"text input: {e.AsString}");
    }

    public override void OnMouseUp(MouseButtonEventArgs e, MouseState mouse)
    {
        // Console.WriteLine($"click: {e.Button}, ({mouse.Position.X}, {mouse.Position.Y}); ");
        var pos = new Vector2(mouse.Position.X, GameContext.ScreenHeight - mouse.Position.Y);
        Click?.Invoke(pos);
    }

    public override void OnMouseMove(MouseMoveEventArgs e, MouseState mouse)
    {
        var pos = new Vector2(mouse.Position.X, GameContext.ScreenHeight - mouse.Position.Y);
        MouseMove?.Invoke(pos);
    }

    // public override void OnMouseWheel(MouseWheelEventArgs e, MouseState mouse)
    // {
    //     Console.WriteLine($"scroll: {e.OffsetY}");
    // }
}