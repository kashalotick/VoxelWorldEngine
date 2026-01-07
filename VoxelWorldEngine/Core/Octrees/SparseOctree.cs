namespace VoxelWorldEngine.Core.Octrees;

public class SparseOctree
{
    private List<OctreeNode> _nodes;
    
    private int _maxDepth = 8;
    
    public int Size => 1 << _maxDepth;
    
    
    
    public SparseOctree()
    {
        _nodes =
        [
            new OctreeNode()
        ];
    }
}