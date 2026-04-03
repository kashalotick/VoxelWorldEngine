using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Engine.UI;

namespace GameApp.Content.Scenes.WorldScene;

public class Pause : UiLayout
{
    public event Action Resume;
    public event Action Save;
    public event Action SaveAndExit;

    private readonly GameContext _gameContext;

    public Pause(GameContext gameContext) : base(gameContext.ScreenWidth, gameContext.ScreenHeight)
    {
        _gameContext = gameContext;
    }

    protected override void Load()
    {
        Add(CreateBackground());
        Add(CreateButtonList());
        
        base.Load();
    }

    private UiElement CreateButtonList()
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var empty = _gameContext.TextureRepository.Get("Empty");

        var list = new ListElement(shader, empty)
        {
            Transform = new RectTransform
            {
                Anchor = (0.5f, 0.5f),
                Pivot = (0.5f, 0.5f),
                Offset = (0, 0),
                Scale = 4,
            },
            Color = ColorStyle.Transparent,
            Gap = 16
        };
        // TODO: world tile ???
        list.AddChild(CreateResume());
        list.AddChild(CreateSave());
        list.AddChild(CreateSaveAndExit());

        return list;
    }


    private UiElement CreateBackground()
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var empty = _gameContext.TextureRepository.Get("Empty");

        var bg = new Background(shader, empty);
        bg.ZIndex = -1;
        bg.Color = ColorStyle.PauseBackground;

        return bg;
    }

    private UiElement CreateResume()
    {
        var button = CreateButton("Resume");
        button.Click += () => Resume?.Invoke();
        return button;
    }

    private UiElement CreateSave()
    {
        var button = CreateButton("Save");
        button.Click += () => Save?.Invoke();
        return button;
    }

    private UiElement CreateSaveAndExit()
    {
        var button = CreateButton("Save & Exit");
        button.Click += () => SaveAndExit?.Invoke();
        return button;
    }

    private Button CreateButton(string label)
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var empty = _gameContext.TextureRepository.Get("Empty");
        var font = _gameContext.FontRepository.Get("Pixel");

        var button = new ButtonWithLabel(shader, empty, font, label);
        button.Transform = new RectTransform
        {
            Width = 120,
            Height = 24,
        };
        button.Color = ColorStyle.White;
        button.HoverColor = ColorStyle.Black;
        button.TextColor = ColorStyle.Black;
        button.TextHoverColor = ColorStyle.White;
        // button.Click += () => OnCreateWorld(slot);
        return button;
    }
}