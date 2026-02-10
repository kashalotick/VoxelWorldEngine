using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.LinearOctree;

public struct LinearOctreeNode
{
    public Voxel Voxel;
    public int ChildrenStartIndex {private set; get;}


    public bool IsLeaf => ChildrenStartIndex < 0;
    public bool IsAir => ChildrenStartIndex == -1;
    public bool IsSolid => ChildrenStartIndex == -2;


    public static LinearOctreeNode Air => new(new(sbyte.MinValue), -1);
    public static LinearOctreeNode Solid => new(new(sbyte.MaxValue), -2);

    
    public LinearOctreeNode(Voxel voxel)
    {
        Voxel = voxel;
        ChildrenStartIndex = -1;
    }
    
    public LinearOctreeNode(Voxel voxel, int childrenStartIndex)
    {
        Voxel = voxel;
        ChildrenStartIndex = childrenStartIndex;
    }

    public void SetSolid()
    {
        ChildrenStartIndex = -2;
    }

    public void SetAir()
    {
        ChildrenStartIndex = -1;
    }
    public int GetChildIndex(int octantIndex)
    {
        return ChildrenStartIndex + octantIndex;
    }
    
    public void SetChildrenStartIndex(int startIndex)
    {
        ChildrenStartIndex = startIndex;
    }
    
}