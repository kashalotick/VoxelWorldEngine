using LearningOpenTK.Core.DTO;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace GameApp.Content.Scenes.WorldScene;

public class Inventory
{

    public int InventorySize;
    private int _selectedSlot
    {
        get;
        set
        {
            field = ((value % InventorySize) + InventorySize) % InventorySize;
            SelectedSlot.Value  = _selectedSlot;
        }
    }

    public Reactive<int> SelectedSlot { get; }
    public BlockId SelectedBlock => InventorySlots[_selectedSlot];

    public BlockId[] InventorySlots;


    public Inventory(BlockId[] inventorySlots)
    {
        SelectedSlot = new Reactive<int>();
        InventorySize = inventorySlots.Length;
        InventorySlots = inventorySlots;
        _selectedSlot = 0;
    }

    public void NextSlot()
    {
        _selectedSlot++;
    }
    public void PreviousSlot()
    {
        _selectedSlot--;
    }
    
}
