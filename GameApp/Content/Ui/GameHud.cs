using GameApp.Content.Ui.Elements;
using GameApp.Utils;
using GameEngine.Content.Ui.StaticDraw;
using GameEngine.Engine.Resources.Fonts;
using GameEngine.Engine.Resources.Shaders;
using GameEngine.Engine.Resources.Textures;
using GameEngine.Engine.UI;
using OpenTK.Mathematics;
using VoxelModule.Core.Raycasting;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

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