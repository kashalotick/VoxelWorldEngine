namespace VoxelModule.Engine.Octree;

public struct Voxel : IEquatable<Voxel>
{
    public BlockId BlockId;
    
    public Voxel(BlockId blockId)
    {
        BlockId = blockId;
    }
    
    public bool IsAir => BlockId == BlockId.Air;
    public bool IsVoid => BlockId == BlockId.Void;

    public bool IsTransparent => BlockId == BlockId.Glass;
    
    public static Voxel Air => new Voxel(BlockId.Air);
    public static Voxel Void => new Voxel(BlockId.Void);
    
    
    public bool Equals(Voxel other)
    {
        return BlockId == other.BlockId;
    }

    public override int GetHashCode()
    {
        return (int)BlockId;
    }
    
    public override string ToString()
    {
        return $"[{BlockId}]";
    }
}