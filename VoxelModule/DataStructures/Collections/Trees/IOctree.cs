using VoxelModule.Core.Serialization;
using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace VoxelModule.DataStructures.Common.Collections.Trees;




public interface IOctree<T> 
{
    int MaxDepth { get; }
    int Size { get; }
    int Count { get; }
    bool IsEmpty { get; }
    void Clear();

    IEnumerable<T> Query(Vector3Int min, Vector3Int max);
    void Accept(IOctreeVisitor<T> visitor);
    T GetData(Vector3Int position);
    // IOctreeNode<T> Root(); // TODO: remove?


    bool SetData(Vector3Int index, T data, Func<T, T, bool>? canReplace);
    void ModifyArea(Vector3Int insertPosition, Vector3Int areaSize, T[] data, Func<T, T, bool>? canReplace);


    OctreeMemento<T> Save();
    void Restore(OctreeMemento<T> memento);
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
    IEnumerable<T> Query(Vector3Int min, Vector3Int max);
}

public interface IOctreeVisitor<T>
{
    void Visit(IOctreeNodeReadonly<T> node);
}