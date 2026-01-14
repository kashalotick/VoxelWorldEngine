namespace VoxelWorldEngine.DataStructures.LinearOctree;

public class LinearOctree
{
    // TODO: make stack with free cells indices (after unsubdivide)
    private List<LinearOctreeNode> _nodes;

    public static readonly int MaxDepth = 8;
    public static int Size => 1 << MaxDepth;
    public int RootIndex => 0;
    public LinearOctreeNode Root => _nodes[RootIndex];

    public LinearOctree()
    {
        _nodes = [LinearOctreeNode.Air];
    }

    public List<LinearOctreeNode> GetNodes() => _nodes;

    /// <summary>
    ///     Compute the index of a node in the linear octree given its position.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <returns>The index of the <b>first leaf node</b> according to position</returns>
    public int GetNodeIndex(Vector3Int.Vector3Int position)
    {
        if (!position.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(position));

        var wayToPosition = OctreeMath.FindWayTo(position, Size);

        var nodeIndex = RootIndex;
        for (int i = 0; i < wayToPosition.Length; i++)
        {
            var node = _nodes[nodeIndex];
            if (node.IsLeaf)
            {
                return nodeIndex;
            }

            var nexChildOctant = wayToPosition[i];
            nodeIndex = node.GetChildIndex(nexChildOctant);
        }

        return nodeIndex;
    }

    /// <summary>
    ///     Returns a node in the linear octree given its position.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <returns>Node struct of the <b>first leaf node</b> according to position</returns>
    public LinearOctreeNode GetNode(Vector3Int.Vector3Int position)
    {
        return _nodes[GetNodeIndex(position)];
    }

    // TODO: make tests
    /// <summary>
    ///     Returns a node in the linear octree.
    /// </summary>
    /// <param name="index">The index in the node list.</param>
    /// <returns>Node struct.</returns>
    public LinearOctreeNode GetNode(int index)
    {
        return _nodes[index];
    }

    /// <summary>
    ///     Sets a new node at the specified position in the linear octree with subdividing leaves.
    /// </summary>
    /// <param name="position">The position in the octree as a <see cref="Vector3Int"/>.</param>
    /// <param name="newNode">The new <see cref="LinearOctreeNode"/> to set.</param>
    public void SetNode(Vector3Int.Vector3Int position, LinearOctreeNode newNode)
    {
        if (!position.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(position));

        var wayToPosition = OctreeMath.FindWayTo(position, Size);

        var nodeIndex = RootIndex;
        for (int i = 0; i < wayToPosition.Length; i++)
        {
            var node = _nodes[nodeIndex];
            if (node.IsLeaf)
            {
                SubdivideNode(nodeIndex);
                node = _nodes[nodeIndex];
            }

            var nexChildOctant = wayToPosition[i];
            nodeIndex = node.GetChildIndex(nexChildOctant);
        }
        
        _nodes[nodeIndex] = newNode;
    }

    public void SetNodeVoxel(Vector3Int.Vector3Int position, Voxel.Voxel voxel)
    {
        var index = GetNodeIndex(position);
        SetNodeVoxel(index, voxel);
    }

    public void SetNodeVoxel(int index, Voxel.Voxel voxel)
    {
        // TODO: Add doc; add test??

        var node = _nodes[index];
        node.Voxel = voxel;
        if (node.IsLeaf)
        {
            if (voxel.Density >= 0) node.SetSolid();
            else node.SetAir();
        }

        _nodes[index] = node;
    }


    /// <summary>
    ///     Mark node as parent and add child nodes into the list.
    /// </summary>
    /// <param name="index">The index in the node list.</param>
    /// <returns>Index of first child</returns>
    /// <exception cref="ArgumentException">If the node is already subdivided</exception>
    public int SubdivideNode(int index)
    {
        var parent = _nodes[index];
        if (!parent.IsLeaf) throw new ArgumentException("Node is already subdivided", nameof(index));

        var startChildIndex = _nodes.Count;

        for (int i = 0; i < 8; i++)
        {
            var child = new LinearOctreeNode(parent.Voxel);
            _nodes.Add(child);
        }

        parent.SetChildrenStartIndex(startChildIndex);
        _nodes[index] = parent;

        return startChildIndex;
    }

    /// <summary>
    ///     Mark node on index as leaf.
    /// </summary>
    /// <param name="index">The index in the node list.</param>
    public void UnsubdivideNode(int index)
    {
        var parent = _nodes[index];

        if (parent.IsLeaf) return;

        parent.SetChildrenStartIndex(-2);
        _nodes[index] = parent;
    }
}