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

    private const int TileWidth = 220;
    private const int TileHeight = 20;

    public WorldTile(
        IShader shader,
        ITexture texture,
        IFont font,
        WorldMeta meta
    ) : base(shader, texture)
    {
        Transform = new RectTransform
        {
            Width = TileWidth,
            Height = TileHeight,
            Scale = 4,
        };
        Color = ColorStyle.Transparent;

        AddWorldName(font, meta.Name);
        AddSubInfo(font, meta.Seed, meta.PlayTime);
        AddPlayButton(shader, texture);
        AddDeleteButton(shader, texture);
    }

    private void AddWorldName(IFont font, string name)
    {
        var label = new DynamicText(Shader, font, name);
        label.Color = new(1);
        label.Transform = new RectTransform
        {
            Width = label.Transform.Width,
            Height = label.Transform.Height,
            Anchor = (0, 1),
            Pivot = (0, 1),
        };
        AddChild(label);
    }
    
    private void AddSubInfo(IFont font, int seed, double playtime)
    {
        var elapsed = (int)playtime;
        var hours = elapsed / 3600;
        var minutes = elapsed % 3600 / 60;
        var seconds = elapsed % 60;
        var label = new StaticText(Shader, font, $"{hours:00}:{minutes:00}:{seconds:00} | {seed}");
        label.Color = ColorStyle.Gray;
        label.Transform = new RectTransform
        {
            Anchor = (0, 0),
            Pivot = (0, 0),
            Scale = 0.75f
        };
        AddChild(label);
    }

    private void AddPlayButton(IShader shader, ITexture texture)
    {
        var button = new Button(shader, texture);
        button.Transform = new RectTransform
        {
            Width = TileHeight,
            Height = TileHeight,
            Anchor = (1, 1),
            Pivot = (1, 1),
        };
        button.Color = new(1);
        button.HoverColor = ColorStyle.GreenLight;
        button.Click += () => Play?.Invoke();
        AddChild(button);
    }

    private void AddDeleteButton(IShader shader, ITexture texture)
    {
        var button = new Button(shader, texture);
        button.Transform = new RectTransform
        {
            Width = TileHeight,
            Height = TileHeight,
            Anchor = (1, 1),
            Pivot = (1, 1),
            Offset = (-(TileHeight + 4), 0),
        };
        button.Color = ColorStyle.White;
        button.HoverColor = ColorStyle.RedLight;
        button.Click += () => Delete?.Invoke();
        AddChild(button);
    }
}