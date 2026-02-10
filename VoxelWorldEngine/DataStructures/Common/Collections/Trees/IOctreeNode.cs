using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctreeNode<T> 
{
    IEnumerable<T> Query(Vector3Int min, Vector3Int max);
    // void Apply()
    T Data { get; set; }
    bool IsLeaf { get; }
    
    
    public int MaxDepth { get; set; }
    public int Depth { get; set; }
    public int Size => 1 << (MaxDepth - Depth);

    public Vector3Int Min { get; set; }
    public Vector3Int Max => Min + Vector3Int.One * (Size - 1);


    IOctreeNode<T>[] Children { get; }
        
    void Split();
    bool Merge();
    bool TryMerge();
}