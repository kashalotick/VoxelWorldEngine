using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public record PlaceBlockCommand(VoxelWorld World, Vector3Int VoxelPositionIndex, BlockId BlockId)
    : ICommand
{

    public virtual void Execute()
    {
        World.PlaceBlock(VoxelPositionIndex, BlockId);
    }
}