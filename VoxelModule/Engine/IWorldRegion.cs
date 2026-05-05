using VoxelModule.Core.Raycasting;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine;

public interface IWorldRegion : IRaycastable
{
    bool PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId);

    void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        Voxel[] data,
        Func<Voxel, Voxel, bool>? canReplace
    );
}