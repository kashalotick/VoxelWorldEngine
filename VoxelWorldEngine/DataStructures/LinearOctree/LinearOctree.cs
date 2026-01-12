namespace VoxelWorldEngine.DataStructures.LinearOctree;

public class LinearOctree
{
    private List<LinearOctreeNode> _nodes;

    public static readonly int MaxDepth = 8;
    public int Size => 1 << MaxDepth;

    public LinearOctree()
    {
        _nodes = [LinearOctreeNode.Air];
    }

    /// <summary>
    ///     Compute the index of a node in the linear octree given its position.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <returns>
    ///     The index of the <b>first leaf node</b> according to position
    /// </returns>
    public int GetNodeIndex(Vector3Int.Vector3Int position)
    {
        // TODO: test

        var firstLeafIndex = ForEachLeaf(position, _ => false);

        return firstLeafIndex;
    }

    /// <summary>
    ///     Returns a node in the linear octree given its position.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <returns>
    ///     Node struct of the <b>first leaf node</b> according to position
    /// </returns>
    public LinearOctreeNode GetNode(Vector3Int.Vector3Int position)
    {
        // TODO: test

        return _nodes[GetNodeIndex(position)];
    }

    /// <summary>
    ///     Sets a new node at the specified position in the linear octree with subdividing leaves.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <param name="newNode">The new <see cref="LinearOctreeNode"/> to set.</param>
    public void SetNode(Vector3Int.Vector3Int position, LinearOctreeNode newNode)
    {
        // TODO: test

        var lastLeafIndex = ForEachLeaf(position, index =>
        {
            SubdivideNode(index);
            return true;
        });

        _nodes[lastLeafIndex] = newNode;
    }
    
    


    /// <summary>
    ///     Iterates over all leaves in the octree on the way to the specified position.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <param name="callback"> Call in every leaf, if return <b>false</b> - break</param>
    /// <returns>Index of the last leaf</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the position is out of the bounds of the octree.</exception>
    public int ForEachLeaf(Vector3Int.Vector3Int position, Func<int, bool> callback)
    {
        // TODO: test; optimize?
        if (!position.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(position));

        var currentIndex = 0;
        var currentSize = Size;

        while (currentSize > 1)
        {
            var node = _nodes[currentIndex];
            if (node.IsLeaf)
            {
                if (!callback(currentIndex)) break;
                node = _nodes[currentIndex];
            }

            currentSize >>= 1;
            // if (currentSize == 0) return node;

            int childOffset = 0;

            if (position.X >= currentSize)
            {
                childOffset |= 4;
                position.X -= currentSize;
            }

            if (position.Y >= currentSize)
            {
                childOffset |= 2;
                position.Y -= currentSize;
            }

            if (position.Y >= currentSize)
            {
                childOffset |= 1;
                position.Y -= currentSize;
            }

            currentIndex = node.ChildrenStartIndex + childOffset;
        }

        return currentIndex;
    }

    public int SubdivideNode(int index)
    {
        var parent = _nodes[index];
        if (!parent.IsLeaf) throw new ArgumentException("Node is already subdivided", nameof(index));
        
        var startChildIndex = _nodes.Count;
        parent.SetChildrenStartIndex(startChildIndex);
        
        _nodes[index] = parent;

        for (int i = 0; i < 8; i++)
        {
            var child = new LinearOctreeNode(parent.Voxel);
            _nodes.Add(child);
        }

        return startChildIndex;
    }

    public void UnsubdivideNode(int index)
    {
        
    }
}