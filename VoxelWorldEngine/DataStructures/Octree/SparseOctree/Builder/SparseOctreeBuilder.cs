namespace VoxelWorldEngine.DataStructures.Octree.SparseOctree.Builder;

public class SparseOctreeBuilder<T>
{
    private IOctreeNode<T> _root;
    
    public IOctreeNode<T> Build()
    {
        return _root;
    }

    public SparseOctreeBuilder<T> AddComposite(T data)
    {
        
        return this;
    }
    public SparseOctreeBuilder<T> AddLeaf(T data)
    {
        
        return this;
    }

    public SparseOctreeBuilder<T> Up()
    {
        
        return this;
    }

}