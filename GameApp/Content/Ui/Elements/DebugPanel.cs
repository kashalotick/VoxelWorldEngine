using GameApp.Utils;
using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Ui.Elements;

public record DebugPanelMaterial(
    IShader Shader,
    ITexture Background,
    IFont Font
);

public class DebugPanel : ListElement
{
    private readonly DebugPanelMaterial _material;
    private DynamicText _aabbText;
    private DynamicText _chunkPositionText;
    private DynamicText _fpsText;

    private DynamicText _playerPositionText;
    private DynamicText _rayIsHitText;


    public DebugPanel(DebugPanelMaterial material) : base(material.Shader, material.Background)
    {
        _material = material;

        Orientation = ListOrientation.Vertical;
        Gap = 4;
        AutoSize = true;
        Color = ColorStyle.Transparent;

        InitDebugTexts();
    }


    private string FormatCoordinates(Vector3 vector)
    {
        return $"{vector.X:F2}, {vector.Y:F2}, {vector.Z:F2}";
    }

    public void UpdateFps(int fps)
    {
        _fpsText.SetTextContent($"FPS: {fps}");
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        _playerPositionText.SetTextContent($"xyz: {FormatCoordinates(position)}");
    }

    public void UpdateChunkPosition(Vector3Int chunkPos)
    {
        _chunkPositionText.SetTextContent($"chunk xyz: {chunkPos}");
    }

    public void UpdateRayHit(RayHit hit)
    {
        _rayIsHitText.SetTextContent($"Ray hit: {hit.IsHit} : {hit.Voxel.BlockId.ToString()}");
        _aabbText.SetTextContent($"AABB: {hit.HitIn.ToVector3Int()} / {hit.HitIn.ToVector3Int() + Vector3Int.One}");
    }


    private void InitDebugTexts()
    {
        _fpsText = CreateTextElement();
        _chunkPositionText = CreateTextElement();
        _playerPositionText = CreateTextElement();
        _rayIsHitText = CreateTextElement();
        _aabbText = CreateTextElement();
    }


    private DynamicText CreateTextElement()
    {
        var text = new DynamicText(_material.Shader, _material.Font, " ");
        // text.Transform.Anchor = new Vector2(0, 1);
        // text.Transform.Scale = 2;
        text.Color = ColorStyle.White;

        AddChild(text);
        return text;
    }
}