using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public record PlaceBlockCommand(IWorldRegion Region, Vector3Int VoxelPositionIndex, BlockId BlockId)
    : ICommand
{
    public virtual void Execute()
    {
        Region.PlaceBlock(VoxelPositionIndex, BlockId);
    }
}