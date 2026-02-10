namespace VoxelWorldEngine.DataStructures.LinearOctree.Iterators.StorageAdapters;

public interface INodeStorageAdapter<T>
{
    void Add(T item);
    T GetAndRemove();
    bool IsEmpty();
    void Clear();
}