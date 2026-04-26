using System.Diagnostics.CodeAnalysis;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

public struct Voxel : IEquatable<Voxel>
{
    public BlockId BlockId;
    
    // TODO: make equality operation
    
    public Voxel(BlockId blockId)
    {
        BlockId = blockId;
    }
    
    public bool IsAir => BlockId == BlockId.Air;
    public bool IsVoid => BlockId == BlockId.Void;
    
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