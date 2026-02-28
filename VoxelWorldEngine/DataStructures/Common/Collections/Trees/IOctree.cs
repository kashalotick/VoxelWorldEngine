using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctree<T>
{
    bool IsEmpty { get; }
    
    void Clear();
    
    // IEnumerable<T> Query(Vector3Int min,  Vector3Int max);
    // void Traverse(IOctreeVisitor<T> visitor);
    // IOctreeNode<T> Root(); // TODO: remove?
    
    
    // void Insert(T data, Vector3Int min, Vector3Int max);
    // bool Remove(Vector3Int position);
    
}

public interface IOctreeNodeReadonly<T>
{
    public int Depth { get; }
    public int Size { get; }

    public Vector3Int Min { get; }
    public Vector3Int Max { get; }
    
    T Data { get; }
    bool IsLeaf { get; }
    
    public IOctreeNodeReadonly<T> GetNeighbor(Vector3Int direction);
}

public interface IOctreeVisitor<T>
{
    void Visit(IOctreeNodeReadonly<T> node);
}