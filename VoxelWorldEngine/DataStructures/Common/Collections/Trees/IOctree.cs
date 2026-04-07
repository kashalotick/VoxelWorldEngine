using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctree<T>
{
    bool IsEmpty { get; }
    
    void Clear();

    IEnumerable<T> Query(Vector3Int min, Vector3Int max);
    void Accept(IOctreeVisitor<T> visitor);

    // IOctreeNode<T> Root(); // TODO: remove?


    // void Insert(T data, Vector3Int min, Vector3Int max);
    // bool Remove(Vector3Int position);

}

public interface IOctreeNode<T> : IOctreeNodeReadonly<T>
{
    
}
public interface IOctreeNodeReadonly<out T>
{
    int Depth { get; }
    int Size { get; }

    Vector3Int MinIndex { get; }
    Vector3Int MaxIndex { get; }
    
    T Data { get; }
    bool IsLeaf { get; }
    
    IOctreeNodeReadonly<T>? GetNeighbor(Vector3Int direction);
}

public interface IOctreeVisitor<T>
{
    void Visit(IOctreeNodeReadonly<T> node);
}