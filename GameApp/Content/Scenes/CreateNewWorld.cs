using GameApp.Content.scenes;
using GameApp.Content.Services;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources;
using OpenTK.Graphics.OpenGL4;

namespace GameApp.Content.Scenes;

public class CreateNewWorld : Scene
{
    private Shader _plainShader;
    private Texture _emptyTexture;
    private Font _pixelFont;
    
    private WorldRepository _repository;
    private int _slot;

    
    public CreateNewWorld(GameContext gameContext, WorldRepository repository, int slot) : base(gameContext)
    {
        _repository = repository;
        _slot = slot;
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

        _plainShader = GameContext.ShaderRepository.Get("plain");
        _emptyTexture = GameContext.TextureRepository.Get("empty");
        _pixelFont = GameContext.FontRepository.Get("Pixel");

        var ui = SOR.Register(new UiLayout());
        
        ui.Add(CreateHeading());
        ui.Add(CreateBackButton());
        
        
        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;
        
        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());

    }



    private UiElement CreateHeading()
    {
        var label = new StaticText(_plainShader, _pixelFont, $"Create world #{_slot}", new(1));
        label.Transform = new RectTransform()
        {
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (32 + 20*4 + 20, -43),
            Scale = 4,
        };
        
        return label;
    }

    private UiElement CreateBackButton()
    {
        var button = new Button(_plainShader, _emptyTexture);
        button.Transform = new RectTransform()
        {
            Width = 20,
            Height = 20,
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (32, -32),
            Scale = 4
        };
        button.Color = new(1);
        button.HoverColor = (0.690f, 0.984f, 0.612f);

        button.Click += () => SceneContext.SetState(new MainMenu(GameContext));

        
        return button;
    }
}