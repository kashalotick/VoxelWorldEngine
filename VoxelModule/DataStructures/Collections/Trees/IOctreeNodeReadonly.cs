using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace VoxelModule.DataStructures.Collections.Trees;

public interface IOctreeNodeReadonly<T> : IVisitable<IOctreeVisitor<T>>
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