namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree;

public class SparseOctreeNode : IOctreeNode<NodeData>
{
    public int MaxDepth => 6;
    public int Depth { get; set; }
    public bool IsLeaf { get; }
    public NodeData Data { get; set; }
    
    private readonly List<IOctreeNode<NodeData?>> _children = new List<IOctreeNode<NodeData?>>();
    private byte _childMask = 0b_0000_0000;
    private int _count;
    

}