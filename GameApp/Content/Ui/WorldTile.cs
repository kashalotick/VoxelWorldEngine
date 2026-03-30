using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Engine.Meshes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Mathematics;

namespace GameApp.Content.Ui;

public class WorldTile : StaticElement
{
    public event Action Play;
    public event Action Delete;


    private IFont _font;
    private string _name;
    private int _seed;
    private double _playtime;

    private UiElement _label;

    public WorldTile(
        IShader shader,
        ITexture texture,
        IFont font,
        string name,
        int seed,
        double playtime
    ) : base(shader, texture)
    {
        Transform = new RectTransform
        {
            Width = 160,
            Height = 20,
            Anchor = (0, 0.5f),
            Pivot = (0, 0),
            Offset = (32, 0),
            Scale = 4,
        };
        Color = new(0.1f);

        _font = font;
        _name = name;
        _seed = seed;
        _playtime = playtime;

        AddWorldName();
        AddWorldSeed();
        AddPlaytime();
        AddPlayButton();
        AddDeleteButton();
    }

    private void AddWorldName()
    {
        var label = new DynamicText(Shader, _font, _name, new(1));
        label.Transform = new RectTransform
        {
            Width = label.Transform.Width,
            Height = label.Transform.Height,
            Anchor = (0, 1),
            Pivot = (0, 1),
        };
        _label = label;
        AddChild(label);
    }

    private void AddWorldSeed()
    {
        var label = new StaticText(Shader, _font, _seed.ToString(), new(0.5f));
        label.Transform = new RectTransform
        {
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (_label.Transform.Width + 4, -label.Transform.Height + label.Transform.Height * 0.75f),
            Scale = 0.75f
        };
        AddChild(label);
    }

    private void AddPlaytime()
    {
        var elapsed = (int)_playtime;
        var hours = elapsed / (60 * 24);
        elapsed -= hours * 60 * 24;
        var minutes = elapsed / (60);
        elapsed -= minutes * 60;
        var seconds = elapsed;

        var playtimeString = $"{hours:00}:{minutes:00}:{seconds:00}";
        var playtime = $"{_playtime}";
        var label = new StaticText(Shader, _font, playtimeString, new(0.5f));
        label.Transform = new RectTransform
        {
            Anchor = (0, 1),
            Pivot = (0, 1),
            Offset = (0, -_label.Transform.Height - 1),
            Scale = 0.75f
        };
        AddChild(label);
    }

    private void AddPlayButton()
    {
        var button = new Button(Shader, Texture);
        button.Transform = new RectTransform()
        {
            Width = 20,
            Height = 20,
            Anchor = (1, 1),
            Pivot = (1, 1),
        };
        button.Color = new(1);
        button.HoverColor = (0.690f, 0.984f, 0.612f);

        button.Click += () => Play?.Invoke();
        AddChild(button);
    }

    private void AddDeleteButton()
    {
        var button = new Button(Shader, Texture);
        button.Transform = new RectTransform()
        {
            Width = 20,
            Height = 20,
            Anchor = (1, 1),
            Pivot = (1, 1),
            Offset = (-24, 0),
        };
        button.Color = (0.937f, 0.259f, 0.259f);
        button.HoverColor = (1.0f, 0.435f, 0.435f);

        button.Click += () => Delete?.Invoke();
        AddChild(button);
    }
}