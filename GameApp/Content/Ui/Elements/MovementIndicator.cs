using GameApp.Content.Controllers.MovementStrategies;
using LearningOpenTK.Core.Transform;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;

namespace GameApp.Content.Ui.Elements;

public record MovementInfoMaterial(
    IShader Shader,
    ITexture WalkIcon,
    ITexture FlyIcon,
    ITexture FreeFlyIcon
);

public class MovementIndicator : DynamicElement
{
    private readonly MovementInfoMaterial _material;
    private MovementMode _movementInfo = MovementMode.Walk;

    public MovementIndicator(MovementInfoMaterial material) : base(material.Shader, material.WalkIcon)
    {
        _material = material;

        Transform = new RectTransform
        {
            Width = 16,
            Height = 16
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