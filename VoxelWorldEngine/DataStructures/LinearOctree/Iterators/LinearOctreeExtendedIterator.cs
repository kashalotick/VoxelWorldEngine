using System.Collections;
using VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators;

public class LinearOctreeExtendedIterator : IEnumerator<LinearOctreeNode>, IEnumerable<LinearOctreeNode>
{
    private const int Children = 8;

    private readonly LinearOctree _octree;
    private INodeStorageAdapter<int> _storage;

    private LinearOctreeNode _current;
    private LinearOctreeNode Current => _current;


    public static IEnumerator<LinearOctreeNode> GetBreathFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeExtendedIterator(octree, new QueueAdapter<int>());
    }

    public IEnumerator<LinearOctreeNode> GetDepthFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeExtendedIterator(octree, new StackAdapter<int>());
    }

    private LinearOctreeExtendedIterator(LinearOctree octree, INodeStorageAdapter<int> storageAdapter)
    {
        _octree = octree;
        _storage = storageAdapter;
        Reset();
    }

    public bool MoveNext()
    {
        while (!_storage.IsEmpty())
        {
            var index = _storage.GetAndRemove();
            var node = _octree.GetNode(index);

            if (node.IsLeaf)
            {
                _current = node;
                return true;
            }

            for (int i = 0; i < Children; i++)
            {
                var childIndex = node.GetChildIndex(i);
                _storage.Add(childIndex);
            }
        }

        return false;
    }

    public void Reset()
    {
        _storage.Clear();
        _storage.Add(_octree.RootIndex);
        _current = default;
    }

    LinearOctreeNode IEnumerator<LinearOctreeNode>.Current => _current;

    object IEnumerator.Current => _current;

    public void Dispose()
    {
        _storage.Clear();
    }

    public IEnumerator<LinearOctreeNode> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class LinearOctreeIterator123(LinearOctree octree)
{
    public IEnumerable<ExtendedLinearOctreeNode> ExtendedBreadthFirst()
    {
        const int children = 8;

        var queue = new Queue<ExtendedLinearOctreeNode>();
        var extendedNodeRoot
            = new ExtendedLinearOctreeNode(octree.Root, octree.RootIndex, 1, Vector3Int.Vector3Int.Zero);

        queue.Enqueue(extendedNodeRoot);

        while (queue.Count > 0)
        {
            var extendedNode = queue.Dequeue();

            if (extendedNode.Node.IsLeaf) yield return extendedNode;

            for (int i = 0; i < children; i++)
            {
                var childIndex = extendedNode.Node.GetChildIndex(i);
                var childNode = octree.GetNode(childIndex);
                var childDepth = extendedNode.Depth + 1;
                var childPosition = extendedNode.Position + OctreeMath.GetOctantCorner(i, 1 << childDepth);

                var extendedNodeChild = new ExtendedLinearOctreeNode(childNode, childIndex, childDepth, childPosition);

                queue.Enqueue(extendedNodeChild);
            }
        }
    }
}