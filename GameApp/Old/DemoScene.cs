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
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core;

namespace GameApp.Old;

public class DemoScene : TestScene
{
    public Func<Shader, Texture, List<WorldObject>> genchunks;
    private ChunkLoadingSystem _chunkLoadingSystem; // temp

    public DemoScene(float screenWidth, float screenHeight, InputProvider inputProvider, Func<Shader, Texture, List<WorldObject>> genMesh) : base(screenWidth, screenHeight, inputProvider)
    {
        genchunks = genMesh;
    }

    public override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        
        FpvCamera = new FPVCamera(new Vector3(1, 2, 3), Size.X / Size.Y); // TODO: make injection for position
        _controller = new CameraController(_inputProvider, FpvCamera);
        _controller.ExitRequested += RequestCloseWindow;

        FpvCamera.LookAt(Vector3.Zero);


        

        // objects
        // var shader = new Shader("shader");
        // var texture = new Texture("Diamond.png");
        // var generateWorldObjectList = genchunks(shader, texture);

        
        // var cubeMesh = Cube.CreateMesh();
        // var cube = new WorldObject(cubeMesh, shader, texture);
        // cube.Transform.Position = new Vector3(0, 0, 0);
        // GameObjectLayout.GameObjects.Add(cube);
        //
        // var cube2 = new WorldObject(cubeMesh, shader, texture);
        // cube2.Transform.Position = new Vector3(64, 0, 64);
        // GameObjectLayout.GameObjects.Add(cube2);

        
        
        // foreach (var worldObject in generateWorldObjectList)
        // {
        //     GameObjectLayout.GameObjects.Add(worldObject);
        // }
        //
        // foreach (var gameObject in GameObjectLayout.GameObjects)
        // {
        //     gameObject.Load();
        // }
        
        var resourceRepository = new ResourceRepository();
        
        var cubeMesh = Cube.CreateMesh();
        var cube = new WorldObject(cubeMesh, resourceRepository.ShaderShader, resourceRepository.DiamondTexture);
        cube.Transform.Position = new Vector3(0, 0, 0);
        GameObjectLayout.GameObjects.Add(cube);
        // GameObjectLayout.Load();
        

        
        // world
        var ws = new WorldService();
        var world = ws.GenerateWorld(124);
        
        var gameWorld = new GameWorld(resourceRepository);
        world.ChunkAdded += gameWorld.AddChunk;
        world.ChunkRemoved += gameWorld.RemoveChunk;
        Entries.Add(gameWorld);
        
        _chunkLoadingSystem = new ChunkLoadingSystem(world);
        
        foreach (var entry in Entries)
        {
            entry.Load();
        }
        // light
        LightComposition(resourceRepository.ShaderShader, resourceRepository.DiamondTexture);

    }

    private static void LightComposition(Shader shader, Texture texture)
    {
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

    public override void FixedUpdate(double deltaTime)
    {
        base.FixedUpdate(deltaTime);

        var player = new Player
        {
            Position = (System.Numerics.Vector3)FpvCamera.Position,
            ViewDirection = (System.Numerics.Vector3)FpvCamera.Front,
            ChunkViewRadius = 3
        };
        _chunkLoadingSystem.Update(player);
    }
}