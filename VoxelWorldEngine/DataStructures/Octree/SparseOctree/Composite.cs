namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree;

public class Composite : IOctreeNode<NodeData>
{
    public int MaxDepth => 6;
    public int Depth { get; set; }
    public bool IsLeaf { get; }
    public NodeData Data { get; set; }
    
    private readonly List<IOctreeNode<NodeData?>> _children = new List<IOctreeNode<NodeData?>>();
    private byte _childMask = 0b_0000_0000;
    private int _count;
    
    
    
    
    public void Insert(int octant, IOctreeNode<NodeData> data)
    {
        var oldData = _children[octant]?.Data;
        
        throw new NotImplementedException();
    }

    public void Remove(int octant)
    {
        throw new NotImplementedException();
    }
}