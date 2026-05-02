using GameApp.Content.Systems;
using GameApp.Utils;
using LearningOpenTK.Content.Ui.StaticDraw;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.UI;

namespace GameApp.Content.Ui.Elements;

public record InventoryPanelMaterial(
    IShader Shader,
    ITexture Background,
    ITexture Selection,
    ITexture[] Slots,
    ITexture Empty
);

public class InventoryPanel : ListElement
{
    private readonly Inventory _inventory;
    private readonly InventoryPanelMaterial _material;

    private readonly Slot[] _slots;

    private int _previousSelectedSlot;

    public InventoryPanel(Inventory inventory, InventoryPanelMaterial material) : base(material.Shader,
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
        for (var i = 0; i < _inventory.InventorySize; i++)
        {
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
        _slots[_previousSelectedSlot].IsSelected = false;
        _slots[newSlot].IsSelected = true;
        _previousSelectedSlot = newSlot;
    }


    private class Slot
    {
        private readonly ITexture _itemTexture;
        private readonly ITexture _selectionTexture;

        private readonly IShader _shader;

        public Slot(int index, IShader shader, ITexture selection, ITexture item)
        {
            _shader = shader;
            _selectionTexture = selection;
            _itemTexture = item;
            Item = CreateBlockSlot();
            Selection = CreateSelection();

            Item.AddChild(Selection);
        }

        public UiElement Selection { get; }
        public UiElement Item { get; }


        public bool IsSelected
        {
            get;
            set
            {
                field = value;
                Selection.IsVisible = value;
            }
        } = false;

        private UiElement CreateSelection()
        {
            var element = new StaticElement(_shader, _selectionTexture)
            {
                Transform =
                {
                    Width = 20,
                    Height = 20,
                    Pivot = (0.5f, 0.5f),
                    Anchor = (0.5f, 0.5f)
                },
                IsVisible = false
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
                    Height = 16
                }
            };

            return element;
        }
    }
}