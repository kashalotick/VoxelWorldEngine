using VoxelModule.Core.Serialization;
using VoxelModule.Core.Trees.LinearImplementation;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Trees;

public partial class Octree<T> : IOctree<T>
{
    public int MaxDepth { get; private set; }
    public int Size => _linearOctree.Size;
    private LinearOctree<T> _linearOctree;

    // private bool isModified = false;

    public Octree()
    {
        MaxDepth = Constants.MaxChunkOctreeDepth;
        _linearOctree = new LinearOctree<T>(MaxDepth);
    }

    public int Count => _linearOctree.Count;
    public bool IsEmpty => _linearOctree.Root.IsLeaf;


    public void Clear()
    {
        _linearOctree = new LinearOctree<T>(MaxDepth);
    }

    public IEnumerable<T> Query(Vector3Int min, Vector3Int max)
    {
        return Root().Query(min, max);
    }

    public void Accept(IOctreeVisitor<T> visitor)
    {
        Root().Accept(visitor);
    }


    internal OctreeNode Root()
    {
        var root = new OctreeNode(_linearOctree, _linearOctree.RootIndex)
        {
            Depth = 0,
            MinIndex = Vector3Int.Zero,
        };
        return root;
    }

    public T GetData(Vector3Int position)
    {
        Counter.Increment(CounterType.VoxelOctreeGetData);

        var nodeIndex = _linearOctree.GetNodeIndex(position);
        var node = _linearOctree.GetNode(nodeIndex);
        return node.Data;
    }

    public bool SetData(Vector3Int index, T data, Func<T, T, bool>? canReplace = null)
    {
        return _linearOctree.SetData(data, index, canReplace);
    }

    public void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        T[] data,
        Func<T, T, bool>? canReplace = null
    )
    {

        // 1. Обчислюємо межі перетину
        // Початок: беремо максимум між 0 та позицією вставки
        int startX = Math.Max(0, insertPosition.X);
        int startY = Math.Max(0, insertPosition.Y);
        int startZ = Math.Max(0, insertPosition.Z);

        // Кінець: беремо мінімум між розміром дерева та кінцем області вставки
        int endX = Math.Min(Size, insertPosition.X + areaSize.X);
        int endY = Math.Min(Size, insertPosition.Y + areaSize.Y);
        int endZ = Math.Min(Size, insertPosition.Z + areaSize.Z);

        // 2. Ітеруємося тільки в межах цього перетину
        for (int z = startZ; z < endZ; z++)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    var worldPos = new Vector3Int(x, y, z);

                    int localX = x - insertPosition.X;
                    int localY = y - insertPosition.Y;
                    int localZ = z - insertPosition.Z;

                    int flatIndex = localX + localY * areaSize.X + localZ * areaSize.X * areaSize.Y;

                    var result = _linearOctree.SetData(data[flatIndex], worldPos, canReplace);
                }
            }
        }
    }


    public OctreeMemento<T> Save()
    {
        return _linearOctree.Save();
    }

    public void Restore(OctreeMemento<T> memento)
    {
        _linearOctree.Restore(memento);
    }
}