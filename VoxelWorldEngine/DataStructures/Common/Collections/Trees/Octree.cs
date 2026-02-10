using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public class Octree<T> : IOctree<T>
{
    public const int MaxDepth = 6;
    private LinearOctree<T> _linearOctree;
    
    private OctreeNode<T> _root;
    
    public bool IsEmpty => _linearOctree.Root.IsLeaf;

    protected IOctreeNode<T> Root()
    {
        var root = new OctreeNode<T>(_linearOctree, _linearOctree.RootIndex)
        {
            MaxDepth = MaxDepth,
            Depth = 0,
            Min = Vector3Int.Zero,
        };
        return root;
    }

    public void Clear()
    {
        _root = null;
    }
}