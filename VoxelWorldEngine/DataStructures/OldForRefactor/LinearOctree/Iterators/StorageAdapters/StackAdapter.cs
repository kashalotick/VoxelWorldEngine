namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

public class StackAdapter<T> : INodeStorageAdapter<T>
{
    private Stack<T> _stack = new Stack<T>();

    public void Add(T item) => _stack.Push(item);
    public T GetAndRemove() => _stack.Pop();
    public bool IsEmpty() => _stack.Count == 0;
    public void Clear() => _stack.Clear();
}