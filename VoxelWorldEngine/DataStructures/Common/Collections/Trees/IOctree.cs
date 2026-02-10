using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public interface IOctree<T> 
    where T : struct
{
    // void Insert(T data, Vector3Int min, Vector3Int max);
    bool Remove(Vector3Int position);
    
    // IEnumerable<T> Query(AABB area);

    
    // void Clear();
}