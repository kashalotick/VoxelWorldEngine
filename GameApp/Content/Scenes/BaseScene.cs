using GameApp.Application;
using GameEngine.Core;
using GameEngine.Engine.Scenes;

namespace GameApp.Content.Scenes;

public class BaseScene : Scene
{
    public BaseScene(MyGameContext gameContext, Camera? camera = null) : base(gameContext, camera)
    {
    }

    protected new MyGameContext GameContext => (MyGameContext)base.GameContext;
}