using GameApp.Content.scenes;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources;
using OpenTK.Graphics.OpenGL4;

namespace GameApp.Content.Scenes;

public class MainMenu : Scene
{
    private Shader _plainShader;
    private Texture _emptyTexture;
    private Font _pixelFont;

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

        _plainShader = GameContext.ShaderRepository.Get("plain");
        _emptyTexture = GameContext.TextureRepository.Get("empty");
        _pixelFont = GameContext.FontRepository.Get("Pixel");

        var ui = SOR.Register(new UiLayout());


        // var fpsText = new DynamicText(_plainShader, _pixelFont, "FPS");
        // fpsText.Transform.Anchor = (.5f, .5f);
        // fpsText.Transform.Pivot = (.5f, .5f);
        // fpsText.Transform.Scale = 2;
        // ui.Add(fpsText);
        // fpsText.SetTextContent("FSPS");
        // fps.OnFpsChanged += f => fpsText.SetTextContent($"FPS: {f}");
        //


        AddExitButton(ui);

        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;

        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }

    private void AddExitButton(UiLayout ui)
    {
        var button = new Button(_plainShader, _emptyTexture);
        button.Transform = new RectTransform()
        {
            Width = 96,
            Height = 48,
            Anchor = (1, 0),
            Pivot = (1, 0),
            Offset = (-32, 32),
            Scale = 2,
        };
        button.Color = (0.937f, 0.259f, 0.259f);
        button.HoverColor = (1.0f, 0.435f, 0.435f);
        button.ZIndex = 1;
        var label = new StaticText(_plainShader, _pixelFont, "Exit", (1, 1, 1));
        label.Transform = new RectTransform()
        {
            Anchor = (1, 0),
            Pivot = (0, 0),
            Offset = (-32-96-40, 32+30),
            Scale = 4,
        };
        label.ZIndex = 2;

        button.Click += () => SceneContext.RequestWindowAction(new CloseWindow());

        ui.Add(button);
        ui.Add(label);
    }
}