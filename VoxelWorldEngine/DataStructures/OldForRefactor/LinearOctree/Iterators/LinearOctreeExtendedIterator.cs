using System.Collections;
using VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators;

public class LinearOctreeExtendedIterator : IEnumerator<ExtendedLinearOctreeNode>, IEnumerable<ExtendedLinearOctreeNode>
{
    private const int Children = 8;

    private readonly LinearOctree _octree;
    private INodeStorageAdapter<ExtendedLinearOctreeNode> _storage;

    private ExtendedLinearOctreeNode _current;
    private ExtendedLinearOctreeNode Current => _current;


    public static IEnumerator<ExtendedLinearOctreeNode> GetBreathFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeExtendedIterator(octree, new QueueAdapter<ExtendedLinearOctreeNode>());
    }

    public IEnumerator<ExtendedLinearOctreeNode> GetDepthFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeExtendedIterator(octree, new StackAdapter<ExtendedLinearOctreeNode>());
    }

    private LinearOctreeExtendedIterator(LinearOctree octree, INodeStorageAdapter<ExtendedLinearOctreeNode> storageAdapter)
    {
        _octree = octree;
        _storage = storageAdapter;
        Reset();
    }

    public bool MoveNext()
    {
        while (!_storage.IsEmpty())
        {
            var extendedNode = _storage.GetAndRemove();

            if (extendedNode.Node.IsLeaf)
            {
                _current = extendedNode;
                return true;
            }

            for (int i = 0; i < Children; i++)
            {
                var childIndex = extendedNode.Node.GetChildIndex(i);
                var childNode = _octree.GetNode(childIndex);
                var childDepth = extendedNode.Depth + 1;
                var childPosition = extendedNode.Position + OctreeMath.GetOctantCorner(i, 1 << childDepth);

                var extendedNodeChild = new ExtendedLinearOctreeNode(childNode, childIndex, childDepth, childPosition);

                _storage.Add(extendedNodeChild);
            }
        }

        return false;
    }

    public void Reset()
    {
        _storage.Clear();
        var extendedNodeRoot
            = new ExtendedLinearOctreeNode(_octree.Root, _octree.RootIndex, 1, Vector3Int.Vector3Int.Zero);
        _storage.Add(extendedNodeRoot);
        _current = default;
    }

    ExtendedLinearOctreeNode IEnumerator<ExtendedLinearOctreeNode>.Current => _current;

    object IEnumerator.Current => _current;

    public void Dispose()
    {
        _storage.Clear();
    }

    public IEnumerator<ExtendedLinearOctreeNode> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

