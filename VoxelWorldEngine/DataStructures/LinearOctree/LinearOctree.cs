namespace VoxelWorldEngine.DataStructures.LinearOctree;

public class LinearOctree
{
    private List<LinearOctreeNode> _nodes;

    public static readonly int MaxDepth = 8;
    public int Size => 1 << MaxDepth;
    public LinearOctreeNode Root => _nodes[0];

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
        if (!position.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(position));

        var currentIndex = 0;
        var currentSize = Size;

        while (currentSize >= 1)
        {
            // Отримуємо копію структури
            var node = _nodes[currentIndex];

            if (node.IsLeaf)
            {
                // Викликаємо колбек. Тут користувач може зробити SubdivideNode(currentIndex)
                bool shouldContinue = callback(currentIndex);
            
                // Якщо користувач повернув false, перериваємо пошук взагалі
                if (!shouldContinue) return currentIndex;

                // ВАЖЛИВО: Оскільки node - це struct (копія), а Subdivide змінив реальні дані в списку,
                // ми мусимо оновити нашу локальну копію.
                node = _nodes[currentIndex];

                // Якщо після колбеку це все ще лист — значить ми дійшли кінця.
                if (node.IsLeaf)
                {
                    return currentIndex;
                }
                // Якщо ж IsLeaf стало false (через Subdivide), ми не робим return, 
                // а йдемо далі по коду вниз, обчислювати offset для дітей.
            }

            currentSize >>= 1;
            if (currentSize == 0) return currentIndex;

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

            // Виправлено Y -> Z
            if (position.Z >= currentSize)
            {
                childOffset |= 1;
                position.Z -= currentSize;
            }

            // Переходимо до дитини
            currentIndex = node.ChildrenStartIndex + childOffset;
        }

        return currentIndex;
    }

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

    public void UnsubdivideNode(int index)
    {
        var parent = _nodes[index];

        if (parent.IsLeaf) return; 
    
        parent.SetChildrenStartIndex(-2);
        _nodes[index] = parent;
    }
}