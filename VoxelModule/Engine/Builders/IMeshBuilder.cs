using VoxelModule.Core.Trees;
using VoxelModule.Engine.Chunks;
using VoxelModule.Engine.Octree;

namespace VoxelModule.Engine.Builders;

public interface IMeshBuilder : IOctreeVisitor<Voxel>
{
    ChunkMeshData Build(IVoxelOctree octree);
}