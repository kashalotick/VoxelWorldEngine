using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine;

public interface IGenerator
{
    Voxel Approximate(Vector3Int min, Vector3Int max);
    bool IsUniform(Vector3Int min, Vector3Int max);
}