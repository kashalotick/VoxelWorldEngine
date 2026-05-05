using VoxelModule.Core.Serialization;

using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Trees;

public interface IOctree<T> : IVisitable<T>
{
    int MaxDepth { get; }
    int Size { get; }
    int Count { get; }
    bool IsEmpty { get; }
    void Clear();

    IEnumerable<T> Query(Vector3Int min, Vector3Int max);
    T GetData(Vector3Int position);
    // IOctreeNode<T> Root(); // TODO: remove?


    bool SetData(Vector3Int index, T data, Func<T, T, bool>? canReplace);

    void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        T[] data,
        Func<T, T, bool>? canReplace
    );


    OctreeMemento<T> Save();
    void Restore(OctreeMemento<T> memento);
}
