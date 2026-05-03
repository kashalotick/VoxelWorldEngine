using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Controllers.MovementStrategies;

public interface IMovementStrategy
{
    void Update(double deltaTime, KeyboardState keyboard, MouseState mouse);
}