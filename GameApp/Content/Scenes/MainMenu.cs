using GameApp.Application;
using GameApp.Content.Controllers;
using GameApp.Content.Services;
using GameApp.Content.Ui.Elements;
using GameApp.Utils;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources;

namespace GameApp.Content.Scenes;

public class MainMenu : BaseScene
{
    private const int SlotCount = 3;
    private const int TileWidth = 170;
    private const int TileHeight = 20;
    private Font _pixelFont;


    private Shader _plainShader;
    private Texture _plainTexture;
    private WorldRepository _repository;

    public MainMenu(MyGameContext gameContext) : base(gameContext)
    {
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
        _plainShader = GameContext.ShaderRepository.Get("plain");
        _plainTexture = GameContext.UiAtlas.Get("Plain");
        _pixelFont = GameContext.FontRepository.Get("Pixel");
        _repository = new WorldRepository();

        var ui = SOR.Register(new UiLayout(GameContext.ScreenWidth, GameContext.ScreenHeight));
        ui.Enable();
        ui.Add(CreateBackground());
        ui.Add(CreateHeading());
        ui.Add(BuildSlotList());
        ui.Add(CreateExitButton());
        ui.Add(CreateBackground());

        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;
        // controller.KeyDown += ui.HandleKeyDown;
        // controller.TextInput += ui.HandleTextInput;

        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }

    private UiElement CreateBackground()
    {
        var bg = new Background(_plainShader, _plainTexture);
        bg.Color = ColorStyle.Background;
        bg.ZIndex = -1;
        return bg;
    }


    private UiElement CreateHeading()
    {
        var label = new StaticText(_plainShader, _pixelFont, "Voxel Game Prototype");
        label.Color = ColorStyle.White;
        label.Transform = new RectTransform
        {
            Anchor = (0.5f, 1),
            Pivot = (0.5f, 1),
            Offset = (0, -64),
            Scale = 8
        };

        return label;
    }

    private ListElement BuildSlotList()
    {
        var worldTileMaterial = new WorldTileMaterial(
            _plainShader,
            _plainTexture,
            GameContext.UiAtlas.Get("Play"),
            GameContext.UiAtlas.Get("Cross"),
            _pixelFont);
        var list = new ListElement(_plainShader, _plainTexture)
        {
            Orientation = ListOrientation.Vertical,
            Gap = 16,
            AutoSize = true,
            Color = ColorStyle.Transparent
        };

        list.Transform = new RectTransform
        {
            Anchor = (0.5f, 0.5f),
            Pivot = (0.5f, 0.5f),
            Offset = (0, -32),
            Scale = 4
        };

        var slots = _repository.GetSlots();

        for (var i = 0; i < SlotCount; i++)
        {
            var slot = i;
            UiElement item;

            if (slots[i] is { } meta)
            {
                var tile = new WorldTile(worldTileMaterial, meta);
                tile.Transform = new RectTransform
                {
                    Width = TileWidth,
                    Height = TileHeight
                };
                tile.Play += () => OnPlay(slot);
                tile.Delete += () => OnDelete(slot);
                item = tile;
            }
            else
            {
                item = CreateEmptySlotButton(slot);
            }

            list.AddChild(item);
        }

        return list;
    }

    private UiElement CreateEmptySlotButton(int slot)
    {
        var button = new ButtonWithLabel(_plainShader, _plainTexture, _pixelFont, "+ Create world");
        button.Transform = new RectTransform
        {
            Width = TileWidth,
            Height = TileHeight
        };
        button.Color = ColorStyle.Transparent;
        button.HoverColor = ColorStyle.White;
        button.TextColor = ColorStyle.White;
        button.TextHoverColor = ColorStyle.Background;
        button.Click += () => OnCreateWorld(slot);
        return button;
    }

    private UiElement CreateExitButton()
    {
        var button = new ButtonWithLabel(_plainShader, _plainTexture, _pixelFont, "Exit");
        button.Transform = new RectTransform
        {
            Width = 48,
            Height = 24,
            Anchor = (1, 0),
            Pivot = (1, 0),
            Offset = (-32, 32),
            Scale = 4
        };
        button.Color = ColorStyle.Transparent;
        button.TextColor = ColorStyle.White;
        button.TextHoverColor = ColorStyle.Black;
        button.HoverColor = ColorStyle.RedLight;
        button.Click += OnExit;
        return button;
    }


    private void OnPlay(int slot)
    {
        var worldInfo = _repository.GetSlots()[slot];
        if (worldInfo is null)
        {
            Console.WriteLine("World not found");
            return;
        }

        SceneContext.SetState(new DemoScene(GameContext, _repository, worldInfo));
    }

    private void OnDelete(int slot)
    {
        _repository.DeleteSlot(slot);
        RebuildUi();
    }

    private void OnCreateWorld(int slot)
    {
        // _repository.CreateSlot(slot, "New world");
        // RebuildUi();
        SceneContext.SetState(new CreateNewWorld(GameContext, _repository, slot));
    }

    private void RebuildUi()
    {
        // Простіше перезайти в сцену — всі ресурси коректно перевантажаться
        SceneContext.SetState(new MainMenu(GameContext));
    }


    private void OnExit()
    {
        SceneContext.RequestWindowAction(new CloseWindow());
    }
}