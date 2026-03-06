using GameApp.Debug;
using LearningOpenTK.Content.Input;
using LearningOpenTK.Core;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameApp.Content.Scenes;

public class PlayerController : FreeCameraController
{
    private RayShooter _rayShooter;
    public PlayerController(Camera camera, RayShooter rayShooter) : base(camera)
    {
        _rayShooter =  rayShooter;
    }
    
    


    public override void OnMouseDown(MouseButtonEventArgs e, MouseState mouse)
    {
        if (e.Button == MouseButton.Left)
        {
            _rayShooter.Trace();
        }
    }
}