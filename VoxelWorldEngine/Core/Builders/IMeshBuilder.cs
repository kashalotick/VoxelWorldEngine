using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Special.Collections;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Builders;

public interface IMeshBuilder : IOctreeVisitor<Voxel>
{
    ChunkMeshData Build(IVoxelOctree octree);
}