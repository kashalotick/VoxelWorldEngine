using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core;

public interface IGenerator
{
    Voxel Approximate(Vector3Int min, Vector3Int max);
    bool IsUniform(Vector3Int min, Vector3Int max);
}