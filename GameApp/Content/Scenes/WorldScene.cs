using LearningOpenTK.Content;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Core;
using OpenTK.Mathematics;

namespace GameApp.Content.Scenes;

public class WorldScene : Scene
{
    private FPVCamera _camera;
    private CameraController _controller;
    
    public WorldScene(GameContext gameContext) : base(gameContext)
    {
        _camera = new FPVCamera(new Vector3(1, 2, 3), GameContext.ScreenWidth / GameContext.ScreenHeight); // TODO: make injection for position
        _controller = new CameraController(GameContext.Input, _camera);
        _controller.ExitRequested += RequestCloseWindow;
        
        
    }

    public override bool IsCursorLocked => true;


    protected override void InternalLoad()
    {
        
    }
}