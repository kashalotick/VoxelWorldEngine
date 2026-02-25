using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public partial class Octree<T> : IOctree<T>
{
    public int MaxDepth { get; private set; }
    public int Size => _linearOctree.Size;
    protected LinearOctree<T> _linearOctree;

    public Octree()
    {
        MaxDepth = Constants.MaxChunkOctreeDepth;
        _linearOctree = new LinearOctree<T>(MaxDepth);
    }

    public bool IsEmpty => _linearOctree.Root.IsLeaf;


    public void Clear()
    {
        _linearOctree = new LinearOctree<T>(MaxDepth);
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

    internal T GetData(Vector3Int position)
    {
        var nodeIndex = _linearOctree.GetNodeIndex(position);
        var node = _linearOctree.GetNode(nodeIndex);
        return node.Data;
    }
}