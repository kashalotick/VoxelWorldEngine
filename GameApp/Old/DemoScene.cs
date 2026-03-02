using System.Numerics;
using GameApp.Content;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Core;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Entities.World.Content;
using LearningOpenTK.Entities.World.LightSources;
using LearningOpenTK.Resources;
using LearningOpenTK.Resources.Interfaces;
using LearningOpenTK.Text;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.Core;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameApp.Old;

public class DemoScene : TestScene
{
    private ChunkLoadingSystem _chunkLoadingSystem; // temp


    public DemoScene(GameContext gameContext) : base(gameContext)
    {
    }

    protected override void InternalLoad()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        
        FpvCamera = new FPVCamera(new Vector3(1, 0, 1), GameContext.ScreenWidth / GameContext.ScreenHeight); // TODO: make injection for position
        Controller = new CameraController(GameContext.Input, FpvCamera);
        Controller.ExitRequested += RequestCloseWindow;

        FpvCamera.LookAt(Vector3.Zero);


        var chunkShader = GameContext.ShaderRepository.Get("chunk");
        var cubeShader = GameContext.ShaderRepository.Get("shader");
        var diamondTexture = GameContext.TextureRepository.Get("Stone");
        
        

        var pixelFont = GameContext.FontRepository.Get("Pixel");
        var textShader = GameContext.ShaderRepository.Get("text");

        _text = new TextObject(pixelFont, textShader);
        _text.Load();
        _text.SetTextContent("Kashalot");
        _text.Transform.Position = new Vector3(0, 0, 0);
        
        
        var cubeMesh = Cube.CreateMesh();
        var cube = new WorldObject(cubeMesh, cubeShader, diamondTexture);
        cube.Transform.Position = new Vector3(0, 0, 0);
        GameObjectLayout.GameObjects.Add(cube);
        

        
        // world
        var ws = new WorldService();
        var world = ws.GenerateWorld(124);
        
        var gameWorld = new GameWorld(world, chunkShader, diamondTexture);
        world.ChunkAdded += gameWorld.AddChunk;
        world.ChunkRemoved += gameWorld.RemoveChunk;
        Entries.Add(gameWorld);
        
        _chunkLoadingSystem = new ChunkLoadingSystem(world);
        _chunkLoadingSystem.Initialize();
        
        foreach (var entry in Entries)
        {
            entry.Load();
        }
        // light
        // LightComposition(resourceRepository.ShaderShader, resourceRepository.DiamondTexture);
        LightComposition(chunkShader, diamondTexture);
        LightComposition(cubeShader, diamondTexture);

        // fps
        InitFpsCounter();

    }
    public override void Render(double deltaTime)
    {
        _fpsCounter.Update(deltaTime);
        
        base.Render(deltaTime);
        GL.Disable(EnableCap.DepthTest);
        _text.Render2D(RenderContext);
    }

    private static void LightComposition(IShader shader, ITexture texture)
    {
        shader.Use();
        
        var sun = new Sun(shader, texture);
        
        sun.Transform.Position = new Vector3(0, 10, 0);
        sun.LightColor = new Vector3(1.0f, 1.0f, 0.95f);
        sun.LightDirection = Vector3.Normalize(new Vector3(-1, -2, -1));
        
        shader.SetVector3("lightColor", sun.LightColor);
        shader.SetVector3("lightDirection", sun.LightDirection);
        
        var ambientColor = new Vector4(0.95f, 0.95f, 1, 0.3f);
        shader.SetVector4("ambientColor", ambientColor);
        var shininess = 64f;
        shader.SetFloat("shininess", shininess);
    }

    private TextObject _text;
    private FpsCounter _fpsCounter;

    private void InitFpsCounter()
    {
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _fpsCounter = new FpsCounter(1.0, 0.25);
        
        var pixelFont = GameContext.FontRepository.Get("Pixel");
        var textShader = GameContext.ShaderRepository.Get("text");
        
        _text = new TextObject(pixelFont, textShader);
        _text.Text.Color = (System.Numerics.Vector3)new Vector3(1, 1, 1);
        _text.Transform.Position = new Vector3(24, 24, 0);
        _text.Transform.Scale = new Vector3(2);
        
        _text.SetTextContent("ABCDEFG absdefg");
        _text.Load();
        _fpsCounter.OnFpsChanged += fps => _text.SetTextContent($"FPS: {fps}");
    }

    public override void FixedUpdate(double deltaTime)
    {
        base.FixedUpdate(deltaTime);

        var player = new Player
        {
            Position = (System.Numerics.Vector3)FpvCamera.Position,
            ViewDirection = (System.Numerics.Vector3)FpvCamera.Front,
            ChunkViewRadius = 4,
            ViewMatrix = (Matrix4x4)FpvCamera.GetViewMatrix()
        };
        _chunkLoadingSystem.Update(deltaTime, player);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _chunkLoadingSystem.Dispose();
    }
}