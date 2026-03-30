using GameApp.Content.scenes;
using GameApp.Content.Ui;
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

    public override void StateEnter()
    {
        ((IScene)this).Load();
    }

    public override void StateExit()
    {
        ((IScene)this).Dispose();
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


        ui.Add(CreateExitButton());
        ui.Add(CreateNewWorldButton());


        var worldTile = new WorldTile(_plainShader, _emptyTexture, _pixelFont, "World", 123432, 23949134);
        worldTile.Play += Play;
        worldTile.Delete += Delete;

        ui.Add(worldTile);


        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;

        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }

    private UiElement CreateExitButton()
    {
        var button = new ButtonWithLabel(_plainShader, _emptyTexture, _pixelFont, "Exit", (1, 1, 1));
        button.Transform = new RectTransform()
        {
            Width = 48,
            Height = 24,
            Anchor = (1, 0),
            Pivot = (1, 0),
            Offset = (-32, 32),
            Scale = 4,
        };
        button.Color = (0.937f, 0.259f, 0.259f);
        button.HoverColor = (1.0f, 0.435f, 0.435f);


        button.Click += Exit;

        return button;
    }

    private UiElement CreateNewWorldButton()
    {
        var button = new ButtonWithLabel(_plainShader, _emptyTexture, _pixelFont, "Create new world", (1, 1, 1));
        button.Transform = new RectTransform()
        {
            Width = 120,
            Height = 24,
            Anchor = (0, 0),
            Pivot = (0, 0),
            Offset = (32, 32),
            Scale = 4,
        };
        button.Color = new(0);
        button.HoverColor = new(0);
        button.TextHoverColor = new(0.8f);


        button.Click += CreateNew;

        return button;
    }

    private void Play()
    {
        SceneContext.SetState(new DemoScene(GameContext));

    }
    private void Delete()
    {
        Console.WriteLine("Delete");
    }

    private void Exit()
    {
        SceneContext.RequestWindowAction(new CloseWindow());
    }

    private void CreateNew()
    {
        SceneContext.SetState(new NewWorld(GameContext));
    }
}