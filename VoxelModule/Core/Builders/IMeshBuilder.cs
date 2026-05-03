using VoxelModule.DataStructures.Collections.Trees;
using VoxelModule.DataStructures.Special.Collections;
using VoxelModule.DataStructures.Special.Collections.Meshes;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Core.Builders;

public interface IMeshBuilder : IOctreeVisitor<Voxel>
{
    ChunkMeshData Build(IVoxelOctree octree);
}