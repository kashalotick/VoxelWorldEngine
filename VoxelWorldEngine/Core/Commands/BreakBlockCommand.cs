using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public record BreakBlockCommand(   IWorldRegion Region, Vector3Int VoxelPositionIndex) : PlaceBlockCommand(Region,
    VoxelPositionIndex, BlockId.Air);