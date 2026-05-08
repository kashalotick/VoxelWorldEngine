using VoxelModule.Core.Raycasting;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine;

public interface IWorldRegion : IRaycastable
{
    bool SetBlock(Vector3Int voxelPositionIndex, BlockId blockId, Func<Voxel, Voxel, bool>? canReplace);
}