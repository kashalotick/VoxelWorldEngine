using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public record BreakBlockCommand(VoxelWorld World, Vector3Int VoxelPositionIndex) : PlaceBlockCommand(World,
    VoxelPositionIndex, BlockId.Air);