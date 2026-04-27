using GameApp.Application;
using GameApp.Content.Controllers;
using GameApp.Content.Services;
using GameApp.Content.Systems;
using GameApp.Content.Ui;
using GameApp.Content.Ui.Elements;
using GameApp.Graphics.VoxelSelectionSystem;
using GameApp.Graphics.World;
using GameApp.Utils;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelWorldEngine.Content.Commands;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace GameApp.Content.Scenes;

public class DemoScene : BaseScene
{
    private const float TextUpdateInterval = 1 / 60f;
    private readonly Reactive<Vector3Int?> _hitVoxelPosition = new();
    private readonly ThrottleReactive<RayHit> _rayHit = new(TextUpdateInterval);

    private readonly WorldRepository _worldRepository;
    private CharacterPhysics _characterPhysics;
    private ChunkLoadingSystem _chunkLoadingSystem;
    private ChunkUpdateSystem _chunkUpdateSystem;
    private CubeCommandFactory _cubeFactory;

    private double _elapsedTime;


    private FpsCounter _fpsCounter;
    private GameWorld _gameWorld;
    private GameHud _hud;
    private Inventory _inventory;
    private MovementIndicator _movementIndicator;
    private Pause _pause;
    private PlayerController _playerController;
    private Raycaster _raycaster;

    private Sky _sky;
    private SphereCommandFactory _sphereFactory;
    private TreeCommandFactory _treeFactory;
    private UiController _uiController;
    private VoxelSelection _voxelSelection;


    private VoxelWorld _voxelWorld;
    private WorldMeta _worldMeta;
    private WorldState _worldState;

    public DemoScene(MyGameContext gameContext, WorldRepository repository, WorldMeta worldMeta) : base(gameContext)
    {
        _worldRepository = repository;
        _worldMeta = worldMeta;
    }

    public override void StateEnter()
    {
        ((IScene)this).Load();
    }

    public override void StateExit()
    {
        ((IScene)this).Dispose();
    }

    protected override void Load()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);        

        LoadSky();
        LoadWorld();
        LoadRaycasting();
        LoadPause();
        LoadHud();
        LoadMainHudPanel();
        LoadCommandFactories();

        Camera = new Camera(_worldState.Player.Position, GameContext.ScreenWidth / GameContext.ScreenHeight);
        var viewDirection = _worldState.Player.ViewDirection == Vector3.Zero
            ? Vector3.UnitX
            : _worldState.Player.ViewDirection;
        Camera.LookAt(_worldState.Player.Position + viewDirection);

        _characterPhysics = new CharacterPhysics(_worldState.Player.Position,
            voxelPosition => _voxelWorld.IsSolid(voxelPosition));
        _playerController = new PlayerController(Camera, _raycaster, _characterPhysics);
        _playerController.Pause += OnPause;
        _playerController.ToggleHud += OnToggleHud;
        _playerController.ToggleDebug += OnToggleDebug;
        _playerController.PlaceBlock += OnPlaceBlock;
        _playerController.BreakBlock += OnBreakBlock;
        _playerController.MiddleButtonClick += OnMiddleButtonClick;
        _playerController.InventoryNext += _inventory.NextSlot;
        _playerController.InventoryPrevious += _inventory.PreviousSlot;
        _playerController.SetBrushSize += _inventory.SetBrushSize;
        _playerController.ToggleBrushType += _inventory.ToggleBrushType;
        _playerController.SetMovementMode += _movementIndicator.UpdatedMovementMode;


        ControllerContext.SetState(_playerController);
        SceneContext.RequestWindowAction(new GrabCursor());
    }

    private void LoadSky()
    {
        var skyTop = new Vector3(0.35f, 0.65f, 0.95f);
        var skyBottom = new Vector3(0.85f, 0.95f, 1.0f);

        _sky = new Sky(GameContext.ShaderRepository.Get("sky"), skyTop, skyBottom);
        SOR.Register(_sky);
    }

    private void LoadWorld()
    {
        new BlockRegistry().Build(GameContext.TextureArrayRepository); // essential

        var material = new GameWorldMaterial(
            GameContext.ShaderRepository.Get("chunk"),
            GameContext.TextureArrayRepository.Get("Blocks"),
            16
        );

        _voxelWorld = _worldRepository.LoadWorld(_worldMeta);
        var worldState = _worldRepository.LoadState(_worldMeta.Slot);
        _worldState = worldState ?? new WorldState();

        _gameWorld = new GameWorld(_voxelWorld, material, _worldRepository.GetChunkRepository(_worldMeta.Slot));
        _voxelWorld.ChunkAdded += _gameWorld.AddChunk;
        _voxelWorld.ChunkUpdated += _gameWorld.UpdateChunk;
        _voxelWorld.ChunkRemoved += _gameWorld.RemoveChunk;
        _gameWorld.Load();

        _chunkLoadingSystem
            = SOR.Register(new ChunkLoadingSystem(_voxelWorld, _worldRepository.GetChunkRepository(_worldMeta.Slot)));
        _chunkUpdateSystem = new ChunkUpdateSystem(_voxelWorld);

        LightComposition(material.Shader);
    }

    private void LoadRaycasting()
    {
        var debugLinesShader = GameContext.ShaderRepository.Get("line");

        _raycaster = new Raycaster(debugLinesShader);
        _raycaster.Load();

        _voxelSelection = SOR.Register(new VoxelSelection(debugLinesShader));
        _hitVoxelPosition.OnChanged += pos => _voxelSelection.SetVoxelPosition(pos);
    }


    private void LoadHud()
    {
        var hudMaterial = new GameHudMaterial(
            GameContext.ShaderRepository.Get("plain"),
            GameContext.UiAtlas.Get("Plain"),
            GameContext.UiAtlas.Get("Crosshair"),
            GameContext.FontRepository.Get("Pixel")
        );
        _hud = SOR.Register(new GameHud(GameContext.ScreenWidth, GameContext.ScreenHeight, hudMaterial));
        _hud.Enable();
        _fpsCounter = new FpsCounter(1.0, 0.25);
        _fpsCounter.OnFpsChanged += fps => _hud.UpdateFps(fps);
        _rayHit.OnChanged += hit => _hud.UpdateRayHit(hit);
    }

    private void LoadMainHudPanel()
    {
        var inventory = LoadInventory();
        var brushInfo = LoadBrushIndicator();
        var movementIndicator = LoadMovementIndicator();

        var hudList = new ListElement(GameContext.ShaderRepository.Get("plain"), GameContext.UiAtlas.Get("Plain"))
        {
            Transform =
            {
                Pivot = new Vector2(0.5f, 0),
                Anchor = new Vector2(0.5f, 0),
                Offset = new Vector2(0, 16),
                Scale = 4
            },
            Orientation = ListOrientation.Horizontal,
            Gap = 16,
            AutoSize = true,
            Color = ColorStyle.Transparent,
            IsReversed = true
        };
        hudList.AddChild(movementIndicator);
        hudList.AddChild(inventory);
        hudList.AddChild(brushInfo);

        _hud.Add(hudList);
    }

    private UiElement LoadInventory()
    {
        BlockId[] inventoryBlocks =
        [
            BlockId.Stone,
            BlockId.Dirt,
            BlockId.Grass,
            BlockId.Bricks,
            BlockId.Glass,
            BlockId.Planks,
            BlockId.Wood,
            BlockId.Leaves
        ];
        _inventory = new Inventory(inventoryBlocks);

        var inventoryHudMaterial = new InventoryPanelMaterial(
            GameContext.ShaderRepository.Get("plain"),
            GameContext.UiAtlas.Get("Plain"),
            GameContext.UiAtlas.Get("Selection"),
            inventoryBlocks.Select(block => (ITexture)GameContext.BlockAtlas.Get(block.ToString())).ToArray(),
            GameContext.UiAtlas.Get("Empty")
        );
        var inventoryHud = new InventoryPanel(_inventory, inventoryHudMaterial);

        return inventoryHud;
    }

    private UiElement LoadBrushIndicator()
    {
        var material = new BrushInfoMaterial(
            GameContext.ShaderRepository.Get("plain"),
            GameContext.FontRepository.Get("Pixel"),
            GameContext.UiAtlas.Get("Cube"),
            GameContext.UiAtlas.Get("Sphere")
        );
        var brushInfo = new BrushIndicator(material);
        _inventory.BrushSize.OnChanged += size => brushInfo.UpdateBrushSize(size);
        _inventory.BrushType.OnChanged += _ => brushInfo.ToggleBrushType();

        return brushInfo;
    }

    private UiElement LoadMovementIndicator()
    {
        var material = new MovementInfoMaterial(
            GameContext.ShaderRepository.Get("plain"),
            GameContext.UiAtlas.Get("Walk"),
            GameContext.UiAtlas.Get("Fly"),
            GameContext.UiAtlas.Get("FreeFly")
        );
        var movementIndicator = new MovementIndicator(material);
        _movementIndicator = movementIndicator;
        return movementIndicator;
    }

    private void LoadPause()
    {
        var pauseMaterial = new PauseMaterial(
            GameContext.ShaderRepository.Get("plain"),
            GameContext.UiAtlas.Get("Plain"),
            GameContext.FontRepository.Get("Pixel")
        );
        _pause = SOR.Register(new Pause(GameContext.ScreenWidth, GameContext.ScreenHeight, pauseMaterial));
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

    private void LoadCommandFactories()
    {
        _treeFactory = new TreeCommandFactory();
        _sphereFactory = new SphereCommandFactory();
        _cubeFactory = new CubeCommandFactory();
    }


    protected override void Render(RenderContext renderContext)
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.DepthTest);
        _sky.Render(renderContext);

        _fpsCounter.Update(renderContext.DeltaTime);
        _gameWorld.Render(renderContext);

        GL.Disable(EnableCap.DepthTest);
        _raycaster.Render(renderContext);
    }

    public override void Update(double deltaTime)
    {
        _elapsedTime += deltaTime;
        ProcessRaycast(deltaTime);
    }

    public override void FixedUpdate(double deltaTime)
    {
        base.FixedUpdate(deltaTime);

        _worldState.Player.ViewDirection = Camera.Front;
        _worldState.Player.Position = Camera.Position;
        _worldState.Player.ViewMatrix = Camera.GetViewMatrix();

        _hud.UpdatePlayerPosition(_worldState.Player.Position);
        _hud.UpdateChunkPosition(Chunk.GlobalToChunk(_worldState.Player.Position.ToVector3Int()));

        _chunkLoadingSystem.Update(deltaTime, _worldState.Player);
        _chunkUpdateSystem.Update(deltaTime);
    }

    private void ProcessRaycast(double deltaTime)
    {
        var ray = new Ray
        {
            Length = 25,
            Origin = (System.Numerics.Vector3)Camera.Position,
            Direction = (System.Numerics.Vector3)Camera.Front
        };

        var rayHit = _raycaster.Shoot(ray, _voxelWorld);

        _hitVoxelPosition.Value = rayHit.IsHit
            ? (rayHit.HitIn - rayHit.HitFaceNormal * 0.001f).FloorToVector3Int()
            : null;

        _rayHit.Value = rayHit;
        _rayHit.Update(deltaTime);
    }

    private static void LightComposition(IShader shader)
    {
        shader.Use();
        shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 0.95f));
        shader.SetVector3("lightDirection", Vector3.Normalize(new Vector3(-1, -2, -1)));
        shader.SetVector4("ambientColor", new Vector4(0.95f, 0.95f, 1f, 0.4f));
        shader.SetFloat("shininess", 64f);
    }

    private void SaveWorld()
    {
        _worldRepository.SaveState(_worldMeta.Slot, _worldState);
        _worldMeta = _worldRepository.UpdateMeta(_worldMeta, _elapsedTime);
        _elapsedTime = 0;
    }

    private void OnToggleDebug()
    {
        _hud.ToggleDebug();
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

    private void OnPlaceBlock()
    {
        var blockToPlace = _inventory.SelectedBlock;

        var lastHit = _raycaster.LastHit;
        if (!lastHit.IsHit) return;

        var hitVoxel = lastHit.HitIn - lastHit.HitFaceNormal * 0.001f;
        var placeVoxel = (hitVoxel + lastHit.HitFaceNormal).FloorToVector3Int();

        Console.WriteLine(
            $"Place block at {placeVoxel}, on normal {lastHit.HitFaceNormal.ToVector3Int()} of {hitVoxel.ToVector3Int()}");
        var command = DispatchCommand(_inventory.BrushSize.Value, _inventory.BrushType.Value, placeVoxel, blockToPlace);
        command.Execute();
    }

    private void OnBreakBlock()
    {
        var lastHit = _raycaster.LastHit;
        if (!lastHit.IsHit) return;

        var hitVoxel = (lastHit.HitIn - lastHit.HitFaceNormal * 0.001f).FloorToVector3Int();

        Console.WriteLine($"Breaking block at {hitVoxel}");
        var command = DispatchCommand(_inventory.BrushSize.Value, _inventory.BrushType.Value, hitVoxel, BlockId.Air);
        command.Execute();
    }

    private ICommand DispatchCommand(int brushSize, BrushShape brashType, Vector3Int position, BlockId blockId)
    {
        if (brushSize == 1)
        {
            return new PlaceBlockCommand(_voxelWorld, position, blockId);
        }

        var modifyMode = blockId == BlockId.Air ? ModifyMode.ReplaceAll : ModifyMode.ReplaceAir;

        switch (brashType)
        {
            case BrushShape.Cube:
                return _cubeFactory.GetCommand(_voxelWorld, position, new CubeArgs(brushSize, blockId, modifyMode));
            case BrushShape.Sphere:
                return _sphereFactory.GetCommand(_voxelWorld, position, new SphereArgs(brushSize, blockId, modifyMode));
            default:
                throw new ArgumentException();
        }
    }

    private void OnMiddleButtonClick()
    {
        var lastHit = _raycaster.LastHit;
        if (!lastHit.IsHit) return;

        var hitVoxel = lastHit.HitIn - lastHit.HitFaceNormal * 0.001f;
        var voxelPlaceIndex = (hitVoxel + lastHit.HitFaceNormal).FloorToVector3Int();
        Console.WriteLine(
            $"Place Tree at {voxelPlaceIndex}, on normal {lastHit.HitFaceNormal.ToVector3Int()} of {hitVoxel.ToVector3Int()}");
        var command = _treeFactory.GetCommand(_voxelWorld, voxelPlaceIndex, new TreeArgs(1));
        command.Execute();
    }

    protected override void ReleaseManagedResources()
    {
        _gameWorld.Dispose();
        SaveWorld();
    }
}