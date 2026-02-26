using LearningOpenTK.Content;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Core;
using OpenTK.Mathematics;

namespace GameApp.Content.Scenes;

public class WorldScene : Scene
{
    private FPVCamera _camera;
    private CameraController _controller;
    
    public WorldScene(float screenWidth, float screenHeight, InputProvider inputProvider) : base(screenWidth, screenHeight, inputProvider)
    {
        _camera = new FPVCamera(new Vector3(1, 2, 3), Size.X / Size.Y); // TODO: make injection for position
        _controller = new CameraController(inputProvider, _camera);
        _controller.ExitRequested += RequestCloseWindow;
        
        
    }

    public override bool IsCursorLocked => true;


    public override void Load()
    {
        
    }
}