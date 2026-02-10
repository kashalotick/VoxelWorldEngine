namespace VoxelWorldEngine.DataStructures.Octree;

public interface IOctreeNode<T>
{
    int MaxDepth { get; }
    int Depth { get; set; }
    bool IsLeaf { get; }
    
    T Data { get; set; }

    
    
    void Insert(int octant, IOctreeNode<T> node);
    void Remove(int octant);
    
    
    
    // IOctreeNode<T> FindNode();
    //
    // IOctreeNode<T> FindFirstLeaf(Vector3Int.Vector3Int position);
    // // void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData data);
    
    
    
    
}

// TODO: separate to IN and OUT interface