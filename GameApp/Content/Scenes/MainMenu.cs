using GameApp.Content.scenes;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.UI;
using OpenTK.Graphics.OpenGL4;

namespace GameApp.Content.Scenes;

public class MainMenu : Scene
{
    public MainMenu(GameContext gameContext) : base(gameContext)
    {
    }

    protected override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        var fps = SOR.Register(new FpsCounter());
        
        var plainShader = GameContext.ShaderRepository.Get("plain");
        var empty = GameContext.TextureRepository.Get("empty");
        var font = GameContext.FontRepository.Get("Pixel");

        var ui = SOR.Register(new UiLayout());



        var fpsText = new DynamicText(plainShader, font, "FPS");
        fpsText.Transform.Anchor = (.5f, .5f);
        fpsText.Transform.Pivot = (.5f, .5f);
        fpsText.Transform.Scale = 2;
        ui.Add(fpsText);
        fpsText.SetTextContent("FSPS");
        fps.OnFpsChanged += f => fpsText.SetTextContent($"FPS: {f}");
        
        
        // var rect = new StaticElement(plainShader, empty)
        // {
        //     Transform =
        //     {
        //         Width = 240,
        //         Height = 60,
        //         Anchor = (0, 1),
        //         Pivot = (0, 1),
        //         Offset = (24, -24),
        //     },
        //     Color = (1, 1, 1),
        // };
        // ui.Add(rect);
        //
        // var button = new Button(plainShader, empty)
        // {
        //     Transform =
        //     {
        //         Width = 120,
        //         Height = 40,
        //         Anchor = (0, 0),
        //         Pivot = (0, 0),
        //         Offset = (120, 24),
        //     },
        //     Color = (0, 1, 1),
        // };
        // ui.Add(button);
    
        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;
        
        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }
}