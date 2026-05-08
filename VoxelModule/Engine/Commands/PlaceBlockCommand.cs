using VoxelModule.Core;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine.Commands;

public record PlaceBlockCommand(IWorldRegion Region, Vector3Int VoxelPositionIndex, BlockId BlockId)
    : ICommand
{
    public virtual void Execute()
    {
        Region.SetBlock(VoxelPositionIndex, BlockId, null);
    }
}