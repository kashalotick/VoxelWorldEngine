using System.Text.RegularExpressions;
using GameApp.Application;
using GameApp.Content.Controllers;
using GameApp.Content.Services;
using GameApp.Utils;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources;

namespace GameApp.Content.Scenes;

public class CreateNewWorld : BaseScene
{
    private const string DefaultWorldName = "New World";
    private const int FormWidth = 180;

    private readonly WorldRepository _repository;
    private readonly int _slot;

    private readonly Reactive<string> _worldName;
    private readonly Reactive<string> _worldSeed;
    private Font _pixelFont;
    private Shader _plainShader;
    private Texture _plainTexture;


    public CreateNewWorld(MyGameContext gameContext, WorldRepository repository, int slot) : base(gameContext)
    {
        _repository = repository;
        _slot = slot;

        _worldName = new Reactive<string>("");
        _worldSeed = new Reactive<string>("");
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
        _plainShader = GameContext.ShaderRepository.Get("plain");
        _plainTexture = GameContext.UiAtlas.Get("Plain");
        _pixelFont = GameContext.FontRepository.Get("Pixel");

        var ui = SOR.Register(new UiLayout(GameContext.ScreenWidth, GameContext.ScreenHeight));
        ui.Enable();
        ui.Add(CreateBackground());
        ui.Add(CreateHeading());
        ui.Add(CreateBackButton());

        ui.Add(CreateForm());


        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;
        controller.KeyDown += ui.HandleKeyDown;
        controller.TextInput += ui.HandleTextInput;

        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }

    private UiElement CreateBackground()
    {
        var bg = new Background(_plainShader, _plainTexture);
        bg.Color = ColorStyle.Background;
        return bg;
    }

    private UiElement CreateForm()
    {
        var list = new ListElement(_plainShader, _plainTexture);
        list.Transform = new RectTransform
        {
            Anchor = (0.5f, 0.5f),
            Pivot = (0.5f, 0.5f),
            Offset = (0, 0),
            Scale = 4
        };
        list.Color = ColorStyle.Transparent;
        list.Gap = 16;
        list.AddChild(CreateWorldNameField());
        list.AddChild(CreateWorldSeedField());
        list.AddChild(CreateSpacingElement(0, 0));
        list.AddChild(CreateSubmitButton());

        return list;
    }

    private UiElement CreateSpacingElement(int width, int height)
    {
        var rect = new StaticElement(_plainShader, _plainTexture);
        rect.Transform = new RectTransform
        {
            Width = width,
            Height = height
        };
        // rect.Color = ColorStyle.Black;
        return rect;
    }

    private TextField CreateField(Reactive<string> reactive)
    {
        var field = new TextField(_plainShader, _plainTexture, _pixelFont, reactive)
        {
            Transform = new RectTransform
            {
                Width = FormWidth,
                Height = 20,
                Anchor = (0.5f, 0),
                Pivot = (0.5f, 0),
                Offset = (0, 0)
            },
            Color = ColorStyle.Field.Background,
            HoverColor = ColorStyle.Field.BackgroundHover,
            FocusColor = ColorStyle.Field.BackgroundFocus,
            ValueColor = ColorStyle.Field.Text,
            PlaceholderColor = ColorStyle.Field.Placeholder
        };
        field.ValueText.Transform.Anchor = (0.5f, 0.5f);
        field.ValueText.Transform.Pivot = (0.5f, 0.5f);
        field.ValueText.Transform.Offset = (0, 2);

        field.PlaceholderText.Transform.Anchor = (0.5f, 0.5f);
        field.PlaceholderText.Transform.Pivot = (0.5f, 0.5f);
        field.PlaceholderText.Transform.Offset = (0, 1);

        return field;
    }

    private TextField CreateWorldNameField()
    {
        var field = CreateField(_worldName);
        field.Placeholder = DefaultWorldName;
        field.SymbolMask = new Regex(@"^[a-zA-Z0-9 _]*$");
        field.MaxLength = 16;


        return field;
    }

    private TextField CreateWorldSeedField()
    {
        var field = CreateField(_worldSeed);
        field.Placeholder = "Random seed";
        field.SymbolMask = new Regex(@"^[0-9]*$");
        field.MaxLength = 9;

        return field;
    }

    private UiElement CreateSubmitButton()
    {
        var button = new ButtonWithLabel(_plainShader, _plainTexture, _pixelFont, "Create world")
        {
            Transform = new RectTransform
            {
                Width = FormWidth,
                Height = 24,
                Anchor = (0.5f, 0),
                Pivot = (0.5f, 0),
                Offset = (0, 0)
            },
            Color = ColorStyle.White,
            HoverColor = ColorStyle.GreenLight,
            TextColor = ColorStyle.Background,
            TextHoverColor = ColorStyle.Background
        };
        button.Click += OnSubmit;
        return button;
    }

    private UiElement CreateHeading()
    {
        var label = new StaticText(_plainShader, _pixelFont, $"Create world #{_slot}");
        label.Color = ColorStyle.White;
        label.Transform = new RectTransform
        {
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (32 + 20 * 4 + 20, -43),
            Scale = 4
        };

        return label;
    }

    private UiElement CreateBackButton()
    {
        var button = new Button(_plainShader, GameContext.UiAtlas.Get("ArrowLeft"));
        button.Transform = new RectTransform
        {
            Width = 20,
            Height = 20,
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (32, -32),
            Scale = 4
        };
        button.Color = ColorStyle.White;
        button.HoverColor = ColorStyle.RedLight;

        button.Click += OnBack;


        return button;
    }

    private void OnSubmit()
    {
        var worldName = _worldName.Value.Length > 0 ? _worldName.Value.Trim() : DefaultWorldName;
        var seed = new Random().Next();

        if (_worldSeed.Value.Length > 0 && int.TryParse(_worldSeed.Value.Trim(), out var parsedSeed))
        {
            seed = parsedSeed;
        }

        _repository.CreateSlot(_slot, worldName, seed);
        SceneContext.SetState(new MainMenu(GameContext));
    }

    private void OnBack()
    {
        SceneContext.SetState(new MainMenu(GameContext));
    }
}