namespace VoxelWorldEngine.DataStructures.Octree;

// separate interface for reading/writing
public class OctreeLeaf : IOctreeNode
{
    public NodeData Data { get; set; }
    
    public IOctreeNode FindFirstLeaf(Vector3Int.Vector3Int position)
    {
        throw new NotImplementedException();
    }

    public void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData node)
    {
        throw new NotImplementedException();
    }
}