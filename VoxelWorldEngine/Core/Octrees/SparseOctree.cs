namespace VoxelWorldEngine.Core.Octrees;

public class SparseOctree
{
    private readonly int _maxDepth = 8;
    private List<OctreeNode> _nodes;


    public SparseOctree()
    {
        _nodes =
        [
            new OctreeNode()
        ];
    }

    public int Size => 1 << _maxDepth;
}