using VoxelWorldEngine.Core.Voxels;

namespace VoxelWorldEngine.Core.Octrees;

public struct OctreeNode
{
    public int ChildrenStartIndex;
    public byte Mask;
    
    public Voxel? Voxel;
    
    
    public bool IsLeaf => Mask == 0;
    

    public OctreeNode(int childrenStartIndex, byte mask)
    {
        ChildrenStartIndex = childrenStartIndex;
        Mask = mask;
    }

    
    public OctreeNode()
    {
        ChildrenStartIndex = 0;
        Mask = 0b_0000_0000;
    }
}