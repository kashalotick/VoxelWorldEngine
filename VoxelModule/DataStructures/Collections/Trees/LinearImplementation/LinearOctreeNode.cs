namespace VoxelModule.DataStructures.Collections.Trees.LinearImplementation;


public struct LinearOctreeNode<T>
{
    public T Data { set; get; }
    public int ChildrenStartIndex { set; get; } // TODO: if octree max size <= 32x32x32 put short


    public bool IsLeaf => ChildrenStartIndex < 0;


    public LinearOctreeNode()
    {
        Data = default;
        ChildrenStartIndex = -1;
    }
    
    public LinearOctreeNode(T data)
    {
        Data = data;
        ChildrenStartIndex = -1;
    }

    public LinearOctreeNode(T data, int childrenStartIndex)
    {
        Data = data;
        ChildrenStartIndex = childrenStartIndex;
    }


    public int GetChildIndex(int octantIndex)
    {
        return ChildrenStartIndex + octantIndex;
    }
    
    public void MarkAsLeaf()
    {
        ChildrenStartIndex = -1;
    }
}