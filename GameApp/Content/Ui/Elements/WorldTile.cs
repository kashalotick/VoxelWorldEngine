using GameApp.Utils;
using GameEngine.Content.Ui.DynamicDraw;
using GameEngine.Content.Ui.DynamicDraw.Interactive;
using GameEngine.Content.Ui.StaticDraw;
using GameEngine.Core.Transform;
using GameEngine.Engine.Resources.Fonts;
using GameEngine.Engine.Resources.Shaders;
using GameEngine.Engine.Resources.Textures;
using GameEngine.Engine.UI;
using OpenTK.Mathematics;

namespace GameApp.Content.Ui.Elements;

public record WorldTileMaterial(
    IShader Shader,
    ITexture Background,
    ITexture PlayButton,
    ITexture DeleteButton,
    IFont TextFont
);

public class WorldTile : StaticElement
{
    private const int TileWidth = 220;
    private const int TileHeight = 20;

    private readonly WorldTileMaterial _material;
    private readonly WorldMeta _meta;

    public WorldTile(
        WorldTileMaterial material,
        WorldMeta meta
    ) : base(material.Shader, material.Background)
    {
        _material = material;
        _meta = meta;

        Transform = new RectTransform
        {
            Width = TileWidth,
            Height = TileHeight,
            Scale = 4
        };
        Color = ColorStyle.Transparent;

        AddWorldName();
        AddSubInfo();
        AddPlayButton();
        AddDeleteButton();
    }

    public event Action Play;
    public event Action Delete;

    private void AddWorldName()
    {
        var label = new DynamicText(Shader, _material.TextFont, _meta.Name);
        label.Color = new Vector4(1);
        label.Transform = new RectTransform
        {
            Width = label.Transform.Width,
            Height = label.Transform.Height,
            Anchor = (0, 1),
            Pivot = (0, 1)
        };
        AddChild(label);
    }

    private void AddSubInfo()
    {
        var elapsed = (int)_meta.PlayTime;
        var hours = elapsed / 3600;
        var minutes = elapsed % 3600 / 60;
        var seconds = elapsed % 60;
        var label = new StaticText(Shader, _material.TextFont, $"{hours:00}:{minutes:00}:{seconds:00} | {_meta.Seed}");
        label.Color = ColorStyle.Gray;
        label.Transform = new RectTransform
        {
            Anchor = (0, 0),
            Pivot = (0, 0),
            Scale = 0.75f
        };
        AddChild(label);
    }

    private void AddPlayButton()
    {
        var button = new Button(Shader, _material.PlayButton);
        button.Transform = new RectTransform
        {
            Width = TileHeight,
            Height = TileHeight,
            Anchor = (1, 1),
            Pivot = (1, 1)
        };
        button.Color = new Vector4(1);
        button.HoverColor = ColorStyle.GreenLight;
        button.Click += () => Play?.Invoke();
        AddChild(button);
    }

    private void AddDeleteButton()
    {
        var button = new Button(Shader, _material.DeleteButton);
        button.Transform = new RectTransform
        {
            Width = TileHeight,
            Height = TileHeight,
            Anchor = (1, 1),
            Pivot = (1, 1),
            Offset = (-(TileHeight + 4), 0)
        };
        button.Color = ColorStyle.White;
        button.HoverColor = ColorStyle.RedLight;
        button.Click += () => Delete?.Invoke();
        AddChild(button);
    }
}