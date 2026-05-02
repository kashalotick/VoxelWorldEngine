using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Core.Commands;

public record PlaceBlockCommand(IWorldRegion Region, Vector3Int VoxelPositionIndex, BlockId BlockId)
    : ICommand
{
    public virtual void Execute()
    {
        Region.PlaceBlock(VoxelPositionIndex, BlockId);
    }
}