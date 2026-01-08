namespace VoxelWorldEngine.DataStructures.SparseVoxelOctree;

public class SparseVoxelOctree
{
    private readonly int _maxDepth = 8;
    private List<SparseVoxelOctreeNode> _nodes;
    
    public int Size => 1 << _maxDepth;

}