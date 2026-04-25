using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;

public class LinearOctree<T>
{
    public int MaxDepth { get; set; }
    public int Count => _nodes.Count;
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

    public int GetNodeIndex(Vector3Int posIndex)
    {
        // if (!posIndex.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(posIndex));
        if (!posIndex.IsInBounds(Size)) return -1;

        Span<int> way = stackalloc int[MaxDepth];
        OctreeMath.FindWayTo(posIndex, Size, way);

        var nodeIndex = RootIndex;
        for (var i = 0; i < way.Length; i++)
        {
            var node = _nodes[nodeIndex];
            if (node.IsLeaf) return nodeIndex;

            nodeIndex = node.GetChildIndex(way[i]);
        }

        return nodeIndex;
    }

    public LinearOctreeNode<T> GetNode(int index)
    {
        return _nodes[index];
    }

    public bool SetData(T data, Vector3Int posIndex)
    {
        if (!posIndex.IsInBounds(Size)) return false;

        Span<int> way = stackalloc int[MaxDepth];
        Span<int> parentIndices = stackalloc int[MaxDepth];

        OctreeMath.FindWayTo(posIndex, Size, way);

        var nodeIndex = RootIndex;
        for (var i = 0; i < way.Length; i++)
        {
            parentIndices[i] = nodeIndex;
            var node = _nodes[nodeIndex];
            if (node.IsLeaf)
            {
                Split(nodeIndex);
            }
            nodeIndex = _nodes[nodeIndex].GetChildIndex(way[i]);
        }

        var targetNode = _nodes[nodeIndex];
        if (targetNode.Data != null && targetNode.Data.Equals(data))
        {
            return false;
        }

        targetNode.Data = data;
        _nodes[nodeIndex] = targetNode;

        for (var i = way.Length - 1; i >= 0; i--)
        {
            var parentIndex = parentIndices[i];
            if (!TryMerge(parentIndex))
            {
                break;
            }
        }

        return true;
    }
    public void ModifyArea(T data, Vector3Int minIndex, Vector3Int maxIndex)
    {
        // split
        // try merge
        throw new NotImplementedException();
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

        int firstChildIdx = parent.ChildrenStartIndex;
        var firstChild = _nodes[firstChildIdx];

        if (!firstChild.IsLeaf) return false;

        for (int i = 1; i < 8; i++)
        {
            var child = _nodes[firstChildIdx + i];

            if (!child.IsLeaf || !EqualityComparer<T>.Default.Equals(child.Data, firstChild.Data))
            {
                return false;
            }
        }

        parent.Data = firstChild.Data;
        parent.MarkAsLeaf();
        _nodes[index] = parent;

        // TODO: Додати індекси дітей у список вільних комірок для повторного використання
        return true;
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