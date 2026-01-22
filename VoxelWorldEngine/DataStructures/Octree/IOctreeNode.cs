namespace VoxelWorldEngine.DataStructures.Octree;

public interface IOctreeNode
{
    NodeData Data { get; set; }
    
    IOctreeNode FindFirstLeaf(Vector3Int.Vector3Int position);
    void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData node);
    
    
    // void Subdivide();
    
    
}