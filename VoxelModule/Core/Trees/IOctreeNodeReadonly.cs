using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Trees;

public interface IOctreeNodeReadonly<T> : IVisitable<T>
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