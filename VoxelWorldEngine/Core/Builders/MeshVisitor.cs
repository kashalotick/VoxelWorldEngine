using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Builders;

public class VoxelOctreeMeshVisitor : IOctreeVisitor<Voxel> {
    
    
    public void Visit(IOctreeNodeReadonly<Voxel> node)
    {
        throw new NotImplementedException();
    }
}