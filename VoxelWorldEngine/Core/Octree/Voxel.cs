using System.Diagnostics.CodeAnalysis;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

public struct Voxel : IEquatable<Voxel>
{
    public BlockId BlockId;
    
    // TODO: make equality operation
    
    public Voxel(BlockId blockId)
    {
        BlockId = blockId;
    }
    
    public bool IsEmpty => BlockId == 0;
    
    
    public static Voxel Empty => new Voxel(BlockId.Air);


    public bool Equals(Voxel other)
    {
        return BlockId == other.BlockId;
    }

    public override int GetHashCode()
    {
        return (int)BlockId;
    }
}