using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;

public class LinearOctree<T>
{
    public int MaxDepth { get; set; }
    private readonly List<LinearOctreeNode<T>> _nodes; // TODO: make stack with free cells indices (after merge)

    public LinearOctree(int maxDepth)
    {
        MaxDepth = maxDepth;
        _nodes = new List<LinearOctreeNode<T>>(2048);
        _nodes.Add(new LinearOctreeNode<T>());
    }

    public int Size => 1 << MaxDepth;
    public int RootIndex => 0;
    public LinearOctreeNode<T> Root => _nodes[RootIndex];

    public List<LinearOctreeNode<T>> GetNodes()
    {
        return _nodes;
    }

    public int GetNodeIndex(Vector3Int position)
    {
        if (!position.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(position));

        Span<int> way = stackalloc int[6];
        OctreeMath.FindWayTo(position, Size, way);

        var nodeIndex = RootIndex;
        for (var i = 0; i < way.Length; i++)
        {
            var node = _nodes[nodeIndex];
            if (node.IsLeaf) return nodeIndex;

            var nexChildOctant = way[i];
            nodeIndex = node.GetChildIndex(nexChildOctant);
        }

        return nodeIndex;
    }

    public LinearOctreeNode<T> GetNode(int index)
    {
        return _nodes[index];
    }
    
    public void SetNodeData(int index, T data)
    {
        var linearOctreeNode = _nodes[index];
        linearOctreeNode.Data = data;
        _nodes[index] = linearOctreeNode;
    }


    public int Split(int index)
    {
        var parent = _nodes[index];
        if (!parent.IsLeaf) throw new ArgumentException("Node is already splitted", nameof(index));

        var startChildIndex = _nodes.Count;

        for (var i = 0; i < 8; i++)
        {
            var child = new LinearOctreeNode<T>(parent.Data);
            _nodes.Add(child);
        }

        parent.ChildrenStartIndex = startChildIndex;
        _nodes[index] = parent;

        return startChildIndex;
    }

    public bool TryMerge(int index)
    {
        var parent = _nodes[index];
        if (parent.IsLeaf) return false;

        LinearOctreeNode<T> previous = _nodes[parent.GetChildIndex(0)];
        var isNodeSame = true;
        for (int i = 1; i < 8; i++)
        {
            var nextIndex = parent.GetChildIndex(i);
            var next = _nodes[nextIndex];
            if (!next.Data.Equals(previous.Data))
            {
                isNodeSame = false;
                break;
            }
            previous = next;
        }

        if (isNodeSame)
        {
            Merge(index); //TODO: may write native 
            return true;
        }
        return false;
    }

    public bool Merge(int index)
    {
        var parent = _nodes[index];
        if (parent.IsLeaf) return false;

        parent.MarkAsLeaf();
        _nodes[index] = parent;
        return true;
    }
    // public void Merge(int index)
    // {
    //     if (_nodes[index].IsLeaf) return;
    //     _nodes[index].MarkAsLeaf();
    // }
    
    public void Clear()
    {
        _nodes.Clear();
    }
}