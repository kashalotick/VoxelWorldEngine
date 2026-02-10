namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree;

public class Octree<T>
{
    int MaxDepth { get; }
    
    private IOctreeNode<T> _root;
}