using VoxelWorldEngine.DataStructures.Special.Collections;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;

namespace VoxelWorldEngine.Core.Builders;

public interface IMeshBuilder
{
    MeshData Build(IVoxelOctree octree);
}