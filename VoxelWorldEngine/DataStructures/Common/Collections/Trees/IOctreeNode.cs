namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctreeNode<T> 
    where T : struct
{
    T Data { get; set; }
    bool IsLeaf { get; }

    
    void Split();
    bool Merge();
    bool TryMerge();
}