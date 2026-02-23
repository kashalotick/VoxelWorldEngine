using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public partial class Octree<T> : IOctree<T>
{
    public const int MaxDepth = 6;
    private LinearOctree<T> _linearOctree;


    public bool IsEmpty => _linearOctree.Root.IsLeaf;


    public void Clear()
    {
        _linearOctree = new LinearOctree<T>();
    }


    internal OctreeNode Root()
    {
        var root = new OctreeNode(_linearOctree, _linearOctree.RootIndex)
        {
            Depth = 0,
            Min = Vector3Int.Zero,
        };
        return root;
    }
}