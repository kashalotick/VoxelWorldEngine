using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Core;

public interface IGenerator
{
    Voxel Approximate(Vector3Int min, Vector3Int max);
    bool IsUniform(Vector3Int min, Vector3Int max);
}