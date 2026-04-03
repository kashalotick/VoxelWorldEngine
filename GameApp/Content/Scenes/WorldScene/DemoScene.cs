using GameApp.Content.scenes;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameApp.Content.VoxelSelectionSystem;
using GameApp.Debug;
using LearningOpenTK.Content;
using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Scenes.WorldScene;

public class DemoScene : Scene
{
    private ChunkLoadingSystem _chunkLoadingSystem;

    private FpsCounter _fpsCounter;
    private PlayerController _playerController;
    private UiController _uiController;
    private GameHud _hud;
    private Pause _pause;

    private const float TextUpdateInterval = 1 / 60f;
    private ThrottleReactive<RayHit> _rayHit = new(TextUpdateInterval);
    private RayShooter _rayShooter;
    private Reactive<Vector3Int?> _hitVoxelPosition = new();
    private VoxelSelection _voxelSelection;

    private VoxelWorld _voxelWorld;
    private GameWorld _gameWorld;

    public DemoScene(GameContext gameContext) : base(gameContext)
    {
    }

    public override void StateEnter() => ((IScene)this).Load();
    public override void StateExit() => ((IScene)this).Dispose();

    protected override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        LoadWorld();
        LoadRaycasting();
        LoadPause();
        LoadHud();

        Camera = new Camera(new Vector3(1, 0, 0), GameContext.ScreenWidth / GameContext.ScreenHeight);
        Camera.LookAt(Vector3.Zero);

        _playerController = new PlayerController(Camera, _rayShooter);
        _playerController.Pause += OnPause;
        _playerController.ToggleHud += OnToggleHud;

        ControllerContext.SetState(_playerController);
        SceneContext.RequestWindowAction(new GrabCursor());
    }


    private void LoadWorld()
    {
        var chunkShader = GameContext.ShaderRepository.Get("chunk");
        var cubeShader = GameContext.ShaderRepository.Get("shader");
        var stoneTexture = GameContext.TextureRepository.Get("Stone");

        _voxelWorld = new WorldRepository().GenerateWorld(124);

        _gameWorld = new GameWorld(_voxelWorld, chunkShader, stoneTexture);
        _voxelWorld.ChunkAdded += _gameWorld.AddChunk;
        _voxelWorld.ChunkRemoved += _gameWorld.RemoveChunk;
        _gameWorld.Load();

        _chunkLoadingSystem = SOR.Register(new ChunkLoadingSystem(_voxelWorld));

        LightComposition(chunkShader, stoneTexture);
        LightComposition(cubeShader, stoneTexture);
    }

    private void LoadRaycasting()
    {
        var debugLinesShader = GameContext.ShaderRepository.Get("line");

        _rayShooter = new RayShooter(debugLinesShader);
        _rayShooter.Load();

        _voxelSelection = SOR.Register(new VoxelSelection(debugLinesShader));
        _hitVoxelPosition.OnChanged += pos => _voxelSelection.SetVoxelPosition(pos);
    }


    private void LoadHud()
    {
        _hud = SOR.Register(new GameHud(GameContext));
        _hud.Enable();
        _fpsCounter = new FpsCounter(1.0, 0.25);
        _fpsCounter.OnFpsChanged += fps => _hud.UpdateFps(fps);
        _rayHit.OnChanged += hit => _hud.UpdateRayHit(hit);
    }

    private void LoadPause()
    {
        _pause = SOR.Register(new Pause(GameContext));
        _pause.Resume += OnResume;
        _pause.Save += OnSave;
        _pause.SaveAndExit += OnSaveAndExit;

        _uiController = new UiController(GameContext);
        _uiController.Click += _pause.HandleClick;
        _uiController.MouseMove += _pause.HandleMouseMove;
        _uiController.KeyDown += HandleKeyDown;

        void HandleKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.Escape && !e.IsRepeat) OnResume();
        }
    }


    protected override void Render(RenderContext renderContext)
    {
        _fpsCounter.Update(renderContext.DeltaTime);
        _gameWorld.Render(renderContext);

        GL.Disable(EnableCap.DepthTest);
        _rayShooter.Render(renderContext);
    }

    public override void Update(double deltaTime)
    {
        ProcessRaycast(deltaTime);
    }

    public override void FixedUpdate(double deltaTime)
    {
        base.FixedUpdate(deltaTime);

        var player = new Player
        {
            Position = Camera.Position,
            ViewDirection = Camera.Front,
            ChunkViewRadius = 4,
            ViewMatrix = Camera.GetViewMatrix()
        };

        _hud.UpdatePlayerPosition(player.Position);
        _hud.UpdateChunkPosition(Chunk.GlobalToChunk(player.Position.ToVector3Int()));

        _chunkLoadingSystem.Update(deltaTime, player);
    }

    private void ProcessRaycast(double deltaTime)
    {
        var ray = new Ray
        {
            Length = 25,
            Origin = (System.Numerics.Vector3)Camera.Position,
            Direction = (System.Numerics.Vector3)Camera.Front
        };

        var rayHit = _rayShooter.Shoot(ray, _voxelWorld);

        _hitVoxelPosition.Value = rayHit.IsHit
            ? (rayHit.HitIn + _rayShooter.PrevRay.Direction * 0.001f).FloorToVector3Int()
            : null;

        _rayHit.Value = rayHit;
        _rayHit.Update(deltaTime);
    }

    private static void LightComposition(IShader shader, ITexture texture)
    {
        shader.Use();
        shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 0.95f));
        shader.SetVector3("lightDirection", Vector3.Normalize(new Vector3(-1, -2, -1)));
        shader.SetVector4("ambientColor", new Vector4(0.95f, 0.95f, 1f, 0.4f));
        shader.SetFloat("shininess", 64f);
    }

    private void SaveWorld()
    {
        Console.WriteLine("Save world");
    }

    private void OnToggleHud()
    {
        if (_hud.IsDisabled) _hud.Enable();
        else _hud.Disable();
    }

    private void OnPause()
    {
        SceneContext.RequestWindowAction(new ResetCursor());
        ControllerContext.SetState(_uiController);
        _hud.Disable();
        _pause.Enable();
    }

    private void OnResume()
    {
        SceneContext.RequestWindowAction(new GrabCursor());

        ControllerContext.SetState(_playerController);
        _pause.Disable();
        _hud.Enable();
    }

    private void OnSave()
    {
        SaveWorld();
    }


    private void OnSaveAndExit()
    {
        SaveWorld();
        SceneContext.SetState(new MainMenu(GameContext));
    }
}