namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree;

public class SparseOctreeNode : IOctreeNode<NodeData>
{
    public int MaxDepth => 6;
    public int Depth { get; set; }
    public bool IsLeaf { get; }
    public NodeData Data { get; set; }
    public void Insert(IOctreeNode<NodeData> node)
    {
        throw new NotImplementedException();
    }

    public void Apply(Bound region, Action<NodeData> operation)
    {
        throw new NotImplementedException();
    }

    public List<IOctreeNode<NodeData>> Query(Bound region)
    {
        throw new NotImplementedException();
    }

    public void Split()
    {
        throw new NotImplementedException();
    }

    public void TryMerge()
    {
        throw new NotImplementedException();
    }

    private readonly List<IOctreeNode<NodeData?>> _children = new List<IOctreeNode<NodeData?>>();
    private byte _childMask = 0b_0000_0000;
    private int _count;
    

}