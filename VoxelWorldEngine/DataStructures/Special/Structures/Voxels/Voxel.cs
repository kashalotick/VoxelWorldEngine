namespace VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

public struct Voxel
{
    public BlockId BlockId;
    
    
    // TODO: make equality operation
    
    public Voxel(BlockId blockId)
    {
        BlockId = blockId;
    }
    
    public bool IsEmpty => BlockId == 0;

}