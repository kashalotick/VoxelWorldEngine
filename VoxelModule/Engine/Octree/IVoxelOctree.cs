using VoxelModule.Core;
using VoxelModule.Core.Trees;

namespace VoxelModule.Engine.Octree;

public interface IVoxelOctree : IOctree<Voxel>
{
    void Build(IGenerator generator);
}