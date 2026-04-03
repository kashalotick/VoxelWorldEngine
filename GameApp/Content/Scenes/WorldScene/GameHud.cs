using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Engine.UI;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Scenes.WorldScene;

public class GameHud : UiLayout
{
    private readonly GameContext _gameContext;

    private DynamicText _playerPositionText;
    private DynamicText _chunkPositionText;
    private DynamicText _rayIsHitText;
    private DynamicText _aabbText;
    private DynamicText _fpsText;


    public GameHud(GameContext gameContext) : base(gameContext.ScreenWidth, gameContext.ScreenHeight)
    {
        _gameContext = gameContext;
    }

    protected override void Load()
    {
        InitBackground();
        InitFpsText();
        InitDebugTexts();
        InitCrosshair();

        base.Load();
    }

    public void UpdateFps(int fps)
        => _fpsText.SetTextContent($"FPS: {fps}");

    public void UpdatePlayerPosition(Vector3 position)
        => _playerPositionText.SetTextContent($"xyz: {position.FancyString()}");

    public void UpdateChunkPosition(Vector3Int chunkPos)
        => _chunkPositionText.SetTextContent($"chunk xyz: {chunkPos}");

    public void UpdateRayHit(RayHit hit)
    {
        _rayIsHitText.SetTextContent($"Ray hit: {hit.IsHit}");
        _aabbText.SetTextContent($"AABB: {hit.HitIn.ToVector3Int()} / {hit.HitIn.ToVector3Int() + Vector3Int.One}");
    }

    private void InitBackground()
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var empty = _gameContext.TextureRepository.Get("Empty");

        Add(new StaticElement(shader, empty)
        {
            Transform =
            {
                Width = 420,
                Height = 256,
                Anchor = (0, 1),
                Pivot = (0, 1),
                Scale = 1,
            },
            ZIndex = 0,
            Color = ColorStyle.Black,
        });
    }

    private void InitFpsText()
        => _fpsText = FastText(new Vector2(24, 24));

    private void InitDebugTexts()
    {
        _playerPositionText = FastText(new Vector2(24, 24 + 2 * 32));
        _chunkPositionText = FastText(new Vector2(24, 24 + 3 * 32));
        _rayIsHitText = FastText(new Vector2(24, 24 + 5 * 32));
        _aabbText = FastText(new Vector2(24, 24 + 6 * 32));
    }

    private void InitCrosshair()
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var crosshairTexture = _gameContext.TextureRepository.Get("Crosshair");

        var crosshair = new Crosshair(shader, crosshairTexture, 16)
        {
            Color = ColorStyle.White
        };
        Add(crosshair);
    }

    private DynamicText FastText(Vector2 position)
    {
        var shader = _gameContext.ShaderRepository.Get("plain");
        var font = _gameContext.FontRepository.Get("Pixel");

        var text = new DynamicText(shader, font, " ");
        text.Transform.Anchor = new Vector2(0, 1);
        text.Transform.Offset = position with { Y = -position.Y - 16 };
        text.Transform.Scale = 2;
        text.Color = ColorStyle.White;

        Add(text);
        return text;
    }
}