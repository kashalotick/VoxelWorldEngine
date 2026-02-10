namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree;

public class Leaf : IOctreeNode<NodeData>
{
    public int MaxDepth => 6;
    public int Depth { get; set; }
    public bool IsLeaf => true;
    public NodeData Data { get; set; }
    
    
    
    
    public void Insert(int octant, IOctreeNode<NodeData> data)
    {
        throw new NotImplementedException();
    }

    public void Remove(int octant)
    {
        throw new NotImplementedException();
    }

    // public IOctreeNode<NodeData> FindFirstLeaf(Vector3Int.Vector3Int position)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData node)
    // {
    //     throw new NotImplementedException();
    // }
}