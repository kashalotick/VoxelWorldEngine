namespace VoxelWorldEngine.DataStructures.Octree;

public interface IOctreeNode
{
    IOctreeNode FindNode(Vector3Int.Vector3Int position);
    void SetNodeData(Vector3Int.Vector3Int position, NodeData node);
    // void Subdivide();
    
    
}