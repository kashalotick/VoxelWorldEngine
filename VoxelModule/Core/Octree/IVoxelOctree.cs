using VoxelModule.Core;
using VoxelModule.DataStructures.Common.Collections.Trees;
using VoxelModule.DataStructures.Special.Structures.Voxels;
using VoxelModule.Core.Raycasting;
using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace VoxelModule.DataStructures.Special.Collections;

public interface IVoxelOctree : IOctree<Voxel>
{
    void Build(IGenerator generator);
}