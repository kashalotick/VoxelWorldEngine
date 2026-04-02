using GameApp.Content.scenes;
using GameApp.Content.Scenes.WorldScene;
using GameApp.Content.Services;
using GameApp.Content.Ui;
using LearningOpenTK.Content;
using LearningOpenTK.Content.Ui.DynamicDraw.Interactive;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.Input;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameApp.Content.Scenes;

public class MainMenu : Scene
{
    private const int SlotCount = 3;
    private const int TileWidth = 170;
    private const int TileHeight = 20;


    private Shader _plainShader;
    private Texture _emptyTexture;
    private Font _pixelFont;
    private WorldRepository _repository;

    public MainMenu(GameContext gameContext) : base(gameContext)
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
        // GL.Enable(EnableCap.DepthTest);
        // GL.Enable(EnableCap.CullFace);
        // GL.Enable(EnableCap.Blend);
        // GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _plainShader = GameContext.ShaderRepository.Get("plain");
        _emptyTexture = GameContext.TextureRepository.Get("empty");
        _pixelFont = GameContext.FontRepository.Get("Pixel");
        _repository = new WorldRepository();

        var ui = SOR.Register(new UiLayout(GameContext.ScreenWidth, GameContext.ScreenHeight));

        ui.Add(CreateBackground());
        ui.Add(CreateHeading());
        ui.Add(BuildSlotList());
        ui.Add(CreateExitButton());

        var controller = new UiController(GameContext);
        controller.Click += ui.HandleClick;
        controller.MouseMove += ui.HandleMouseMove;
        controller.KeyDown += ui.HandleKeyDown;
        controller.TextInput += ui.HandleTextInput;

        ControllerContext.SetState(controller);
        SceneContext.RequestWindowAction(new ResetCursor());
    }

    private UiElement CreateBackground()
    {
        var bg = new BackGround(_plainShader, _emptyTexture);

        bg.Color = ColorStyle.Background;

        return bg;
    }

    private UiElement CreateHeading()
    {
        var label = new StaticText(_plainShader, _pixelFont, $"Voxel Game Prototype", new(1));
        label.Transform = new RectTransform()
        {
            Anchor = (0.5f, 1),
            Pivot = (0.5f, 1),
            Offset = (0, -64),
            Scale = 8,
        };

        return label;
    }

    private ListElement BuildSlotList()
    {
        var list = new ListElement(_plainShader, _emptyTexture)
        {
            Orientation = ListOrientation.Vertical,
            Gap = 16,
            AutoSize = true,
            Color = new(0)
        };

        list.Transform = new RectTransform
        {
            Anchor = (0, 0.5f),
            Pivot = (0, 0.5f),
            Offset = (32, -32),
            Scale = 4,
        };

        var slots = _repository.GetSlots();

        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i;
            UiElement item;

            if (slots[i] is { } info)
            {
                var tile = new WorldTile(_plainShader, _emptyTexture, _pixelFont, info);
                tile.Transform = new RectTransform
                {
                    Width = TileWidth,
                    Height = TileHeight,
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
        var button = new ButtonWithLabel(_plainShader, _emptyTexture, _pixelFont, "+ Create world");
        button.Transform = new RectTransform
        {
            Width = TileWidth,
            Height = TileHeight,
        };
        button.Color = new(0);
        button.HoverColor = new(1);
        button.TextColor = new(1);
        button.TextHoverColor = new(0);
        button.Click += () => OnCreateWorld(slot);
        return button;
    }

    private UiElement CreateExitButton()
    {
        var button = new ButtonWithLabel(_plainShader, _emptyTexture, _pixelFont, "Exit");
        button.Transform = new RectTransform
        {
            Width = 48,
            Height = 24,
            Anchor = (1, 0),
            Pivot = (1, 0),
            Offset = (-32, 32),
            Scale = 4,
        };
        button.Color = (0.937f, 0.259f, 0.259f);
        button.TextColor = new(1);
        button.HoverColor = (1.0f, 0.435f, 0.435f);
        button.Click += OnExit;
        return button;
    }


    private void OnPlay(int slot)
    {
        SceneContext.SetState(new DemoScene(GameContext));
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