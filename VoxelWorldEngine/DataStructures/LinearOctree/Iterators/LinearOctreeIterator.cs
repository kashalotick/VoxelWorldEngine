using System.Collections;
using VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators;

public class LinearOctreeIterator : IEnumerator<LinearOctreeNode>, IEnumerable<LinearOctreeNode>
{
    private const int Children = 8;

    private readonly LinearOctree _octree;
    private INodeStorageAdapter<int> _storage;

    private LinearOctreeNode _current;
    private LinearOctreeNode Current => _current;


    public static IEnumerator<LinearOctreeNode> GetBreathFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeIterator(octree, new QueueAdapter<int>());
    }

    public IEnumerator<LinearOctreeNode> GetDepthFirstIterator(LinearOctree octree)
    {
        return new LinearOctreeIterator(octree, new StackAdapter<int>());
    }

    private LinearOctreeIterator(LinearOctree octree, INodeStorageAdapter<int> storageAdapter)
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
