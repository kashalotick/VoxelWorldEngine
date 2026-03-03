using System.Numerics;
using GameApp.Content;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Scenes;
using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Entities.World.Content;
using LearningOpenTK.Resources.Interfaces;
using LearningOpenTK.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameApp.Old;

public class DemoScene : Scene
{
    private ChunkLoadingSystem _chunkLoadingSystem; // temp
    protected CameraController Controller;

    
    private GameWorld _gameWorld;

    public DemoScene(GameContext gameContext) : base(gameContext)
    {
    }

    public override bool IsCursorLocked => true;

    protected override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        

        
        Camera = new Camera(new Vector3(1, 0, 1), GameContext.ScreenWidth / GameContext.ScreenHeight); // TODO: make injection for position
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
        var world = ws.GenerateWorld(124);
        
        var gameWorld = new GameWorld(world, chunkShader, diamondTexture);
        world.ChunkAdded += gameWorld.AddChunk;
        world.ChunkRemoved += gameWorld.RemoveChunk;
        _gameWorld = gameWorld;
        _gameWorld.Load();
        // SOR.Register(gameWorld);
        
        _chunkLoadingSystem = new ChunkLoadingSystem(world);
        SOR.Register(_chunkLoadingSystem);
        

        LightComposition(chunkShader, diamondTexture);
        LightComposition(cubeShader, diamondTexture);

        // fps
        InitFpsCounter();

    }
    protected override void Render(RenderContext renderContext)
    {
        _fpsCounter.Update(renderContext.DeltaTime);
        _gameWorld.Render(renderContext);
        GL.Disable(EnableCap.DepthTest);
    }

    private static void LightComposition(IShader shader, ITexture texture)
    {
        shader.Use();
        
        
        var lightColor = new Vector3(1.0f, 1.0f, 0.95f);
        var lightDirection = Vector3.Normalize(new Vector3(-1, -1, -1));

        shader.SetVector3("lightColor", lightColor);
        shader.SetVector3("lightDirection", lightDirection);

        var ambientColor = new Vector4(0.95f, 0.95f, 1, 0.1f);
        shader.SetVector4("ambientColor", ambientColor);
        var shininess = 64f;
        shader.SetFloat("shininess", shininess);
    }

    private FpsCounter _fpsCounter;

    private void InitFpsCounter()
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _fpsCounter = new FpsCounter(1.0, 0.25);
        
        var pixelFont = GameContext.FontRepository.Get("Pixel");
        var textShader = GameContext.ShaderRepository.Get("text");

        var text = SOR.Register(new TextObject(pixelFont, textShader));
        text.Text.Color = (System.Numerics.Vector3)new Vector3(1, 1, 1);
        text.Transform.Position = new Vector3(24, 24, 0);
        text.Transform.Scale = new Vector3(2);
        
        text.SetTextContent("ABCDEFG absdefg");
        _fpsCounter.OnFpsChanged += fps => text.SetTextContent($"FPS: {fps}");
    }

    public override void KeyDown(Keys key)
    {
        Controller.ProcessInput(key);
    }

    public override void Update(double deltaTime)
    {
        Controller.ProcessMouseStreamInput();
        Controller.ProcessKeyboardStreamInput((float)deltaTime);
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
        _chunkLoadingSystem.Update(deltaTime, player);
    }
}