using System.Numerics;
using GameApp.Content;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameApp.Debug;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Resources.Interfaces;
using LearningOpenTK.Text;
using LearningOpenTK.UI.Components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameApp.Old;

public class DemoScene : Scene
{
    private ChunkLoadingSystem _chunkLoadingSystem; // temp
    private TextObject _chunkPositionText;
    private FpsCounter _fpsCounter;

    private const float TextUpdateInterval = 1 / 60f;
    private ThrottleReactive<RayHit> _rayHit = new(TextUpdateInterval);
    private RayShooter _rayShooter;
    
    private World _world;
    private GameWorld _gameWorld;
    private TextObject _playerPositionText;
    protected CameraController Controller;

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


        Camera = new Camera(new Vector3(1, 0, 0),
            GameContext.ScreenWidth / GameContext.ScreenHeight); // TODO: make injection for position
        Controller = new CameraController(GameContext.Input, Camera);
        Controller.ExitRequested += RequestCloseWindow;

        Camera.LookAt(Vector3.Zero);


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
        InitFpsCounter();
        InitPositionTracker();
        InitRaycastTracker();
        InitCrosshair();

        _rayShooter = new RayShooter(GameContext.ShaderRepository.Get("debugray"));
        _rayShooter.Load();
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

        var ambientColor = new Vector4(0.95f, 0.95f, 1, 0.3f);
        shader.SetVector4("ambientColor", ambientColor);
        var shininess = 64f;
        shader.SetFloat("shininess", shininess);
    }

    private void InitRaycastTracker()
    {
        var rayInText = FastText(new Vector3(24, 128, 0));
        var rayOutText = FastText(new Vector3(24, 128 + 32, 0));
        var rayIsHitText = FastText(new Vector3(24, 128 + 64, 0));
        var aabb = FastText(new Vector3(24, 128 + 96, 0));


        _rayHit.OnChanged += hit =>
        {
            aabb.SetTextContent($"AABB: {hit.HitIn.ToVector3Int()} / {hit.HitIn.ToVector3Int() + Vector3Int.One}");
            rayIsHitText.SetTextContent($"Ray hit: {hit.IsHit}");
            rayOutText.SetTextContent($"Ray in: {hit.HitIn.FancyString()}");
            rayInText.SetTextContent($"Ray out: {hit.HitOut.FancyString()}");
        };

    }

    private void InitPositionTracker()
    {
        _chunkPositionText = FastText(new Vector3(24, 24 + 2 * 32, 0));
        _playerPositionText = FastText(new Vector3(24, 24 + 32, 0));
    }


    private void InitCrosshair()
    {
        var textShader = GameContext.ShaderRepository.Get("text");
        var crosshairTexture = GameContext.TextureRepository.Get("Crosshair");
        var crosshair = SOR.Register(new Crosshair(crosshairTexture, textShader));
        crosshair.Color = new Vector3(1, 1, 1);
        crosshair.Size = 16;
    }
    private void InitFpsCounter()
    {
        _fpsCounter = new FpsCounter(1.0, 0.25);


        var text = FastText(new Vector3(24, 24, 0));
        _fpsCounter.OnFpsChanged += fps => text.SetTextContent($"FPS: {fps}");
    }

    private TextObject FastText(Vector3 position)
    {
        var pixelFont = GameContext.FontRepository.Get("Pixel");
        var textShader = GameContext.ShaderRepository.Get("text");

        var text = SOR.Register(new TextObject(pixelFont, textShader));
        text.Transform.Position = position;
        text.Transform.Scale = new Vector3(2);
        return text;
    }

    public override void KeyDown(Keys key)
    {
        Controller.ProcessInput(key);
    }
    

    public override void Update(double deltaTime)
    {
        Controller.ProcessMouseStreamInput();
        Controller.ProcessKeyboardStreamInput((float)deltaTime);

        var ray = new Ray
        {
            Length = 25,
            Origin = (System.Numerics.Vector3)Camera.Position,
            Direction = (System.Numerics.Vector3)Camera.Front
        };
        var rayHit = _world.Raycast(ray);
        
        if (GameContext.Input.Mouse.IsButtonPressed(MouseButton.Left))
        {
            _rayShooter.Update(ray, rayHit);
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