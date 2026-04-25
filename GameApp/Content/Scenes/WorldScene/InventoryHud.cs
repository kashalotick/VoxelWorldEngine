using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;

namespace GameApp.Content.Scenes.WorldScene;

public record InventoryHudMaterial(
    IShader Shader,
    ITexture Background,
    ITexture Selection,
    ITexture[] Slots,
    ITexture Empty
);

public class InventoryHud : ListElement
{
    private InventoryHudMaterial _material;
    private Inventory _inventory;

    private Slot[] _slots;

    private int _previousSelectedSlot = 0;

    public InventoryHud(Inventory inventory, InventoryHudMaterial material) : base(material.Shader,
        material.Background)
    {
        _inventory = inventory;
        _inventory.SelectedSlot.OnChanged += UpdateSelection;
        _material = material;

        Orientation = ListOrientation.Horizontal;
        Gap = 4;
        AutoSize = true;
        Color = ColorStyle.Transparent;
        
        IsReversed = true;

        _slots = new Slot[_inventory.InventorySize];

        InitBlockSlots();
        UpdateSelection(_inventory.SelectedSlot);
    }

    private void InitBlockSlots()
    {
        for (int i = 0; i < _inventory.InventorySize; i++)
        {
            Console.WriteLine($"{i}/{_slots.Length}");
            if (i < _material.Slots.Length)
            {
                _slots[i] = new Slot(i, _material.Shader, _material.Selection, _material.Slots[i]);
            }
            else
            {
                _slots[i] = new Slot(i, _material.Shader, _material.Selection, _material.Empty);
            }
            AddChild(_slots[i].Item);

        }
    }

    private void UpdateSelection(int newSlot)
    {
        Console.WriteLine($"{_inventory.SelectedBlock}, {_inventory.SelectedSlot.Value}");
        _slots[_previousSelectedSlot].IsSelected = false;
        _slots[newSlot].IsSelected = true;
        _previousSelectedSlot = newSlot;
    }


    private class Slot
    {
        public UiElement Selection { get; set; }
        public UiElement Item { get; set; }


        public bool IsSelected
        {
            get;
            set
            {
                field = value;
                Selection.IsVisible = value;
            }
        } = false;

        private IShader _shader;
        private ITexture _itemTexture;
        private ITexture _selectionTexture;

        public Slot(int index, IShader shader, ITexture selection, ITexture item)
        {
            _shader =  shader;
            _selectionTexture = selection;
            _itemTexture = item;
            Item = CreateBlockSlot();
            Selection = CreateSelection();

            Item.AddChild(Selection);
        }

        private UiElement CreateSelection()
        {
            var element = new StaticElement(_shader, _selectionTexture)
            {
                Transform =
                {
                    Width = 20,
                    Height = 20,
                    Pivot = (0.5f, 0.5f),
                    Anchor = (0.5f, 0.5f),
                },
                IsVisible =  false
            };
            return element;
        }

        private UiElement CreateBlockSlot()
        {
            var element = new StaticElement(_shader, _itemTexture)
            {
                Transform =
                {
                    Width = 16,
                    Height = 16,
                }
            };

            return element;
        }
    }
}