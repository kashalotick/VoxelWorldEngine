using GameApp.Content.Controllers.MovementStrategies;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;

namespace GameApp.Content.Ui;

public record MovementInfoMaterial(
    IShader Shader,
    ITexture WalkIcon,
    ITexture FlyIcon,
    ITexture FreeFlyIcon
);

public class MovementIndicator : DynamicElement
{
    private MovementMode _movementInfo = MovementMode.Walk;

    private MovementInfoMaterial _material;

    public MovementIndicator(MovementInfoMaterial material) : base(material.Shader, material.WalkIcon)
    {
        _material = material;
        
        Transform = new RectTransform
        {
            Width = 16,
            Height = 16,
        };
    }

    public void UpdatedMovementMode(MovementMode mode)
    {
        _movementInfo = mode;
        switch (_movementInfo)
        {
            case MovementMode.Walk:
                SetTexture(_material.WalkIcon);
                break;
            case MovementMode.Flight:
                SetTexture(_material.FlyIcon);
                break;
            case MovementMode.FreeCamera:
                SetTexture(_material.FreeFlyIcon);
                break;
        }

    }
}