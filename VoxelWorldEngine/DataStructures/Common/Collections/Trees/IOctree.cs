namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctree<T>
{
    bool IsEmpty { get; }
    
    void Clear();
    // IOctreeNode<T> Root(); // TODO: remove?
    
    
    // void Insert(T data, Vector3Int min, Vector3Int max);
    // bool Remove(Vector3Int position);
    
    // IEnumerable<T> Query(AABB area);
}