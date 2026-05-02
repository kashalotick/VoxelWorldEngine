using VoxelModule.Core.Raycasting;
using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Core;

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