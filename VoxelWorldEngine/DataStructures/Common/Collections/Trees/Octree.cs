using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public partial class Octree<T> : IOctree<T>
{

    public int MaxDepth { get; private set; }
    public int Size => _linearOctree.Size;
    private LinearOctree<T> _linearOctree;
    
    // private bool isModified = false;

    public Octree()
    {
        MaxDepth = Constants.MaxChunkOctreeDepth;
        _linearOctree = new LinearOctree<T>(MaxDepth);
    }

    public int Count => _linearOctree.Count;
    public bool IsEmpty => _linearOctree.Root.IsLeaf;


    public void Clear()
    {
        _linearOctree = new LinearOctree<T>(MaxDepth);
    }

    public IEnumerable<T> Query(Vector3Int min, Vector3Int max)
    {
        return Root().Query(min, max);
    }

    public void Accept(IOctreeVisitor<T> visitor)
    {
        Root().AcceptRecursive(visitor);
    }


    internal OctreeNode Root()
    {
        var root = new OctreeNode(_linearOctree, _linearOctree.RootIndex)
        {
            Depth = 0,
            MinIndex = Vector3Int.Zero,
        };
        return root;
    }

    public T GetData(Vector3Int position)
    {
        Counter.Increment(CounterType.VoxelOctreeGetData);

        var nodeIndex = _linearOctree.GetNodeIndex(position);
        var node = _linearOctree.GetNode(nodeIndex);
        return node.Data;

    }

    public bool SetData(Vector3Int index, T data)
    {
        return _linearOctree.SetData(data, index);
    }

    public void ModifyArea(Vector3Int minIndex, Vector3Int maxIndex, T data)
    {
        _linearOctree.ModifyArea(data, minIndex, maxIndex);
    }
}