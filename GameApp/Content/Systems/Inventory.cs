using GameEngine.Core.Reactive;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace GameApp.Content.Systems;

public enum BrushShape
{
    Cube,
    Sphere
}

public class Inventory
{
    public BlockId[] InventorySlots;


    public Inventory(BlockId[] inventorySlots)
    {
        SelectedSlot = new Reactive<int>();
        InventorySize = inventorySlots.Length;
        InventorySlots = inventorySlots;
        _selectedSlot = 0;
    }

    public Reactive<int> BrushSize { get; } = new(1);
    public Reactive<BrushShape> BrushType { get; } = new(BrushShape.Cube);
    public int InventorySize { get; }

    private int _selectedSlot
    {
        get;
        set
        {
            field = (value % InventorySize + InventorySize) % InventorySize;
            SelectedSlot.Value = _selectedSlot;
        }
    }

    public Reactive<int> SelectedSlot { get; }
    public BlockId SelectedBlock => InventorySlots[_selectedSlot];

    public void NextSlot()
    {
        _selectedSlot++;
    }

    public void PreviousSlot()
    {
        _selectedSlot--;
    }

    public void SetBrushSize(int size)
    {
        BrushSize.Value = size;
    }

    public void ToggleBrushType()
    {
        BrushType.Value = BrushType.Value switch
        {
            BrushShape.Cube => BrushShape.Sphere,
            BrushShape.Sphere => BrushShape.Cube,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}