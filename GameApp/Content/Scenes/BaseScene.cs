using GameApp.Application;
using LearningOpenTK.Core;
using LearningOpenTK.Engine.Scenes;

namespace GameApp.Content.Scenes;

public class BaseScene : Scene
{
    public BaseScene(MyGameContext gameContext, Camera? camera = null) : base(gameContext, camera)
    {
    }

    protected new MyGameContext GameContext => (MyGameContext)base.GameContext;
}