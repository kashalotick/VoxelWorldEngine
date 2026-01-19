namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

public class QueueAdapter<T> : INodeStorageAdapter<T>
{
    private Queue<T> _queue = new Queue<T>();

    public void Add(T item) => _queue.Enqueue(item);
    public T GetAndRemove() => _queue.Dequeue();
    public bool IsEmpty() => _queue.Count == 0;
    public void Clear() => _queue.Clear();
}