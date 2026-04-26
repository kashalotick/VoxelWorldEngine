using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public interface IMovementStrategy
{
    MovementMode Mode { get; }
    void OnEnter();
    void OnExit();
    void Update(double deltaTime, KeyboardState keyboard, MouseState mouse);
}