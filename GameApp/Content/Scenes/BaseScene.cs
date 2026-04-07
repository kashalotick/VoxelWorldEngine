using LearningOpenTK.Core;
using LearningOpenTK.Core.Scenes;

namespace GameApp.Content.Scenes;

public class BaseScene : Scene
{
    protected new MyGameContext GameContext => (MyGameContext)base.GameContext;
    
    public BaseScene(MyGameContext gameContext, Camera? camera = null) : base(gameContext, camera)
    {
    }
}