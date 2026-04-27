using GameApp.Content.Systems;
using GameApp.Utils;
using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Mathematics;

namespace GameApp.Content.Ui.Elements;

public record BrushInfoMaterial(
    IShader Shader,
    IFont Font,
    ITexture CubeIcon,
    ITexture SphereIcon
);

public class BrushIndicator : DynamicElement
{
    private readonly BrushInfoMaterial _material;
    private BrushShape _brushShape = BrushShape.Cube;
    private int _brushSize = 1;
    private DynamicText _text;

    public BrushIndicator(BrushInfoMaterial material) : base(material.Shader, material.CubeIcon)
    {
        _material = material;

        Transform = new RectTransform
        {
            Width = 16,
            Height = 16
        };

        InitBrushSizeText();
    }

    public void UpdateBrushSize(int size)
    {
        _brushSize = size;
        _text.SetTextContent($"{_brushSize}");
    }

    public void ToggleBrushType()
    {
        _brushShape = _brushShape switch
        {
            BrushShape.Cube => BrushShape.Sphere,
            BrushShape.Sphere => BrushShape.Cube,
            _ => throw new ArgumentOutOfRangeException()
        };
        switch (_brushShape)
        {
            case BrushShape.Cube:
                SetTexture(_material.CubeIcon);
                break;
            case BrushShape.Sphere:
                SetTexture(_material.SphereIcon);
                break;
        }
    }

    private void InitBrushSizeText()
    {
        _text = new DynamicText(_material.Shader, _material.Font, $"{_brushSize}")
        {
            Transform = new RectTransform
            {
                Anchor = new Vector2(0, 0.25f),
                Offset = new Vector2(20, 0)
            }
        };
        _text.Color = ColorStyle.White;

        AddChild(_text);
    }
}