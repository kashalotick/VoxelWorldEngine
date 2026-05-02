using GameApp.Content.Ui.Elements;
using GameApp.Utils;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace GameApp.Content.Ui;

public record GameHudMaterial(
    IShader Shader,
    ITexture Background,
    ITexture Crosshair,
    IFont TextFont
);

public class GameHud : UiLayout
{
    private readonly GameHudMaterial _material;

    private DebugPanel _debugPanel;

    public GameHud(float width, float height, GameHudMaterial material) : base(width, height)
    {
        _material = material;
    }

    protected override void Load()
    {
        InitDebugText();
        InitCrosshair();

        base.Load();
    }

    public void UpdateFps(int fps)
    {
        _debugPanel.UpdateFps(fps);
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        _debugPanel.UpdatePlayerPosition(position);
    }

    public void UpdateChunkPosition(Vector3Int chunkPos)
    {
        _debugPanel.UpdateChunkPosition(chunkPos);
    }

    public void UpdateRayHit(RayHit hit)
    {
        _debugPanel.UpdateRayHit(hit);
    }

    public void ToggleDebug()
    {
        _debugPanel.IsVisible = !_debugPanel.IsVisible;
    }

    public void InitDebugText()
    {
        var debugMaterial = new DebugPanelMaterial(
            _material.Shader,
            _material.Background,
            _material.TextFont
        );
        _debugPanel = new DebugPanel(debugMaterial)
        {
            Transform =
            {
                Anchor = (0, 1),
                Pivot = (0, 1),
                Offset = (24, -24),
                Scale = 2
            },
            IsVisible = false
        };
        Add(_debugPanel);
    }


    private void InitCrosshair()
    {
        var crosshair = new Crosshair(_material.Shader, _material.Crosshair, 16)
        {
            Color = ColorStyle.White
        };
        Add(crosshair);
    }
}