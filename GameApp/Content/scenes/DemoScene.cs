using System.Numerics;
using GameApp.Content;
using GameApp.Content.Scenes;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameApp.Content.VoxelSelectionSystem;
using GameApp.Debug;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Input;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Content.Ui;
using LearningOpenTK.Content.Ui.DynamicDraw;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using LearningOpenTK.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameApp.Old;

public class DemoScene : Scene
{
    private ChunkLoadingSystem _chunkLoadingSystem; // temp

    private FpsCounter _fpsCounter;

    private const float TextUpdateInterval = 1 / 60f;
    private ThrottleReactive<RayHit> _rayHit = new(TextUpdateInterval);
    private RayShooter _rayShooter;
    private Reactive<Vector3Int?> _hitVoxelPosition = new();
    private VoxelSelection _voxelSelection;

    private World _world;
    private GameWorld _gameWorld;

    private DynamicText _chunkPositionText;
    private DynamicText _playerPositionText;

    public DemoScene(GameContext gameContext) : base(gameContext)
    {
    }

    public override bool IsCursorLocked => true;

    protected override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        // GL.Enable(EnableCap.Multisample);



        var chunkShader = GameContext.ShaderRepository.Get("chunk");
        var cubeShader = GameContext.ShaderRepository.Get("shader");
        var diamondTexture = GameContext.TextureRepository.Get("Stone");


        // var cubeMesh = Cube.CreateMesh();
        // var cube = new WorldObject(cubeMesh, cubeShader, diamondTexture);
        // cube.Transform.Position = new Vector3(0, 0, 0);
        // SOR.Register(cube);


        // world
        var ws = new WorldService();
        _world = ws.GenerateWorld(124);

        var gameWorld = new GameWorld(_world, chunkShader, diamondTexture);
        _world.ChunkAdded += gameWorld.AddChunk;
        _world.ChunkRemoved += gameWorld.RemoveChunk;
        _gameWorld = gameWorld;
        _gameWorld.Load();
        // SOR.Register(gameWorld);

        _chunkLoadingSystem = new ChunkLoadingSystem(_world);
        SOR.Register(_chunkLoadingSystem);


        LightComposition(chunkShader, diamondTexture);
        LightComposition(cubeShader, diamondTexture);

        // text
        InitDebugText();
        InitCrosshair();
        // var textShader = GameContext.ShaderRepository.Get("text");
        // var font = GameContext.FontRepository.Get("Pixel");
        // var staticText = SOR.Register(new StaticText(textShader, font, "Hello world!")
        // {
        //     Transform =
        //     {
        //         Scale = 4.25f,
        //         Anchor = new Vector2(0.5f, 1f),
        //         Pivot = new Vector2(0.5f, 0.5f),
        //         Offset = new Vector2(0, -32)
        //     },
        //     Color = new Vector3(1, 1, 1)
        // });

        // raycasting
        var debugLinesShader = GameContext.ShaderRepository.Get("debugray");
        _rayShooter = new RayShooter(debugLinesShader);
        _rayShooter.Load();
        _voxelSelection = SOR.Register(new VoxelSelection(debugLinesShader));
        _hitVoxelPosition.OnChanged += voxelPosition => _voxelSelection.SetVoxelPosition(voxelPosition);
        
        
        
        Camera = new Camera(new Vector3(1, 0, 0),
            GameContext.ScreenWidth / GameContext.ScreenHeight); // TODO: make injection for position
        Camera.LookAt(Vector3.Zero);
        
        ControllerContext.SetState(new PlayerController(Camera, _rayShooter));

    }

    protected override void Render(RenderContext renderContext)
    {
        _fpsCounter.Update(renderContext.DeltaTime);
        _gameWorld.Render(renderContext);
        GL.Disable(EnableCap.DepthTest);

        _rayShooter.Render(renderContext);
    }

    private static void LightComposition(IShader shader, ITexture texture)
    {
        shader.Use();


        var lightColor = new Vector3(1.0f, 1.0f, 0.95f);
        var lightDirection = Vector3.Normalize(new Vector3(-1, -2, -1));

        shader.SetVector3("lightColor", lightColor);
        shader.SetVector3("lightDirection", lightDirection);

        var ambientColor = new Vector4(0.95f, 0.95f, 1, 0.4f);
        shader.SetVector4("ambientColor", ambientColor);
        var shininess = 64f;
        shader.SetFloat("shininess", shininess);
    }


    private void InitCrosshair()
    {
        var textShader = GameContext.ShaderRepository.Get("text");
        var crosshairTexture = GameContext.TextureRepository.Get("Crosshair");
        var crosshair = SOR.Register(new Crosshair(textShader, crosshairTexture, 16));
        crosshair.Color = new Vector3(1, 1, 1);
    }


    private void InitDebugText()
    {
        var textShader = GameContext.ShaderRepository.Get("text");
        var empty = GameContext.TextureRepository.Get("Empty");
        SOR.Register(new StaticElement(textShader, empty)
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
            Color = (0, 0, 0),
        });
        
        InitFpsCounter();

        _playerPositionText = FastText(new Vector2(24, 24 + 2 * 32));
        _chunkPositionText = FastText(new Vector2(24, 24 + 3 * 32));

        var rayIsHitText = FastText(new Vector2(24, 24 + 5 * 32));
        var aabb = FastText(new Vector2(24, 24 + 6 * 32));


        _rayHit.OnChanged += hit =>
        {
            aabb.SetTextContent($"AABB: {hit.HitIn.ToVector3Int()} / {hit.HitIn.ToVector3Int() + Vector3Int.One}");
            rayIsHitText.SetTextContent($"Ray hit: {hit.IsHit}");
        };
    }

    private void InitFpsCounter()
    {
        _fpsCounter = new FpsCounter(1.0, 0.25);


        var text = FastText(new Vector2(24, 24));
        _fpsCounter.OnFpsChanged += fps => text.SetTextContent($"FPS: {fps}");
    }

    private DynamicText FastText(Vector2 position)
    {
        var pixelFont = GameContext.FontRepository.Get("Pixel");
        var textShader = GameContext.ShaderRepository.Get("text");

        var text = SOR.Register(new DynamicText(textShader, pixelFont, " "));

        text.Transform.Anchor = new Vector2(0, 1);
        text.Transform.Offset = position with { Y = -position.Y - 16 };
        text.Transform.Scale = 2;
        return text;
    }


    public override void Update(double deltaTime)
    {
        // ProcessRaycast(deltaTime);
    }

    private void ProcessRaycast(double deltaTime)
    {
        var ray = new Ray
        {
            Length = 25,
            Origin = (System.Numerics.Vector3)Camera.Position,
            Direction = (System.Numerics.Vector3)Camera.Front
        };
        var rayHit = _rayShooter.Shoot(ray, _world);
        
        if (rayHit.IsHit)
        {
            var insidePoint = rayHit.HitIn + _rayShooter.PrevRay.Direction * 0.001f;
            _hitVoxelPosition.Value = insidePoint.FloorToVector3Int();
        }
        else
        {
            _hitVoxelPosition.Value = null;
        }

        _rayHit.Value = rayHit;
        _rayHit.Update(deltaTime);
    }


    public override void FixedUpdate(double deltaTime)
    {
        base.FixedUpdate(deltaTime);

        var player = new Player
        {
            Position = (System.Numerics.Vector3)Camera.Position,
            ViewDirection = (System.Numerics.Vector3)Camera.Front,
            ChunkViewRadius = 4,
            ViewMatrix = (Matrix4x4)Camera.GetViewMatrix()
        };
        var chunkPos = Chunk.GlobalToChunk(player.Position.ToVector3Int());
        _playerPositionText.SetTextContent($"xyz: {player.Position.FancyString()}");
        _chunkPositionText.SetTextContent($"chunk xyz: {chunkPos}");

        _chunkLoadingSystem.Update(deltaTime, player);
    }
}