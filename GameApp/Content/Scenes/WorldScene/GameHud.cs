using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Scenes.WorldScene;

public record GameHudMaterial(
    IShader Shader,
    ITexture Background,
    ITexture Crosshair,
    IFont TextFont
);

public class GameHud : UiLayout
{
    private readonly GameHudMaterial _material;

    private DebugHud _debugHud;

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

    public void UpdateFps(int fps) => _debugHud.UpdateFps(fps);

    public void UpdatePlayerPosition(Vector3 position) => _debugHud.UpdatePlayerPosition(position);

    public void UpdateChunkPosition(Vector3Int chunkPos) =>  _debugHud.UpdateChunkPosition(chunkPos);

    public void UpdateRayHit(RayHit hit) => _debugHud.UpdateRayHit(hit);

    public void ToggleDebug()
    {
        _debugHud.IsVisible = !_debugHud.IsVisible;
    }

    public void InitDebugText()
    {
        var debugMaterial = new DebugHudMaterial(
            _material.Shader,
            _material.Background,
            _material.TextFont
        );
        _debugHud = new DebugHud(debugMaterial)
        {
            Transform =
            {
                Anchor = (0, 1),
                Pivot = (0, 1),
                Offset = (24, -24),
                Scale = 2,
            },
            IsVisible = false
        };
        Add(_debugHud);
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