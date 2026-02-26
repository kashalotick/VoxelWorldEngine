using LearningOpenTK.Core;
using LearningOpenTK.Core.Interfaces;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Scenes;

public class BaseScene : IScene
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool IsCursorLocked { get; }
    public void Load()
    {
        throw new NotImplementedException();
    }

    public void KeyDown(Keys key)
    {
        throw new NotImplementedException();
    }

    public void Update(double deltaTime)
    {
        throw new NotImplementedException();
    }

    public void FixedUpdate(double deltaTime)
    {
        throw new NotImplementedException();
    }

    public void Render(double deltaTime)
    {
        throw new NotImplementedException();
    }

    public void Resize(int width, int height)
    {
        throw new NotImplementedException();
    }
}