using GameApp.Utils;
using GameEngine.Content.Ui.DynamicDraw.Interactive;
using GameEngine.Content.Ui.StaticDraw;
using GameEngine.Core.Transform;
using GameEngine.Engine.Resources.Fonts;
using GameEngine.Engine.Resources.Shaders;
using GameEngine.Engine.Resources.Textures;
using GameEngine.Engine.UI;

namespace GameApp.Content.Ui;

public record PauseMaterial(
    IShader Shader,
    ITexture Background,
    IFont TextFont
);

public class Pause : UiLayout
{
    private readonly PauseMaterial _material;

    public Pause(float width, float height, PauseMaterial material) : base(width, height)
    {
        _material = material;
    }

    public event Action Resume;
    public event Action Save;
    public event Action SaveAndExit;


    protected override void Load()
    {
        Add(CreateBackground());
        Add(CreateButtonList());

        base.Load();
    }

    private UiElement CreateButtonList()
    {
        var list = new ListElement(_material.Shader, _material.Background)
        {
            Transform = new RectTransform
            {
                Anchor = (0.5f, 0.5f),
                Pivot = (0.5f, 0.5f),
                Offset = (0, 0),
                Scale = 4
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
        var bg = new Background(_material.Shader, _material.Background);
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
        var button = new ButtonWithLabel(_material.Shader, _material.Background, _material.TextFont, label);
        button.Transform = new RectTransform
        {
            Width = 120,
            Height = 24
        };
        button.Color = ColorStyle.White;
        button.HoverColor = ColorStyle.Black;
        button.TextColor = ColorStyle.Black;
        button.TextHoverColor = ColorStyle.White;
        // button.Click += () => OnCreateWorld(slot);
        return button;
    }
}