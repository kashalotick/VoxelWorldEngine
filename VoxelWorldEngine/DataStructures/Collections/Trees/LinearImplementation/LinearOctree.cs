using VoxelWorldEngine.Core.Serialization;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;

public class LinearOctree<T>
{
    public int MaxDepth { get; set; }
    public int Count => _nodes.Count - _freeClusters.Count * 8;
    private readonly Stack<int> _freeClusters = new();
    private readonly List<LinearOctreeNode<T>> _nodes; // TODO: make stack with free cells indices (after merge)

    public LinearOctree(int maxDepth)
    {
        MaxDepth = maxDepth;
        _nodes = new List<LinearOctreeNode<T>>(2048);
        _nodes.Add(new LinearOctreeNode<T>());
    }

    public int Size => 1 << MaxDepth;
    public int RootIndex => 0;
    public LinearOctreeNode<T> Root => _nodes[RootIndex];

    public List<LinearOctreeNode<T>> GetNodes()
    {
        return _nodes;
    }

    public int GetNodeIndex(Vector3Int posIndex)
    {
        // if (!posIndex.IsInBounds(Size)) throw new ArgumentOutOfRangeException(nameof(posIndex));
        if (!posIndex.IsInBounds(Size)) return -1;

        Span<int> way = stackalloc int[MaxDepth];
        OctreeMath.FindWayTo(posIndex, Size, way);

        var nodeIndex = RootIndex;
        for (var i = 0; i < way.Length; i++)
        {
            var node = _nodes[nodeIndex];
            if (node.IsLeaf) return nodeIndex;

            nodeIndex = node.GetChildIndex(way[i]);
        }

        return nodeIndex;
    }

    public LinearOctreeNode<T> GetNode(int index)
    {
        return _nodes[index];
    }

    public bool SetData(T newData, Vector3Int posIndex, Func<T, T, bool>? canReplace = null)
    {
        if (!posIndex.IsInBounds(Size)) return false;

        Span<int> way = stackalloc int[MaxDepth];
        Span<int> parentIndices = stackalloc int[MaxDepth];

        OctreeMath.FindWayTo(posIndex, Size, way);

        var nodeIndex = RootIndex;
        for (var i = 0; i < way.Length; i++)
        {
            parentIndices[i] = nodeIndex;
            var node = _nodes[nodeIndex];
            if (node.IsLeaf)
            {
                Split(nodeIndex);
            }

            nodeIndex = _nodes[nodeIndex].GetChildIndex(way[i]);
        }

        var targetNode = _nodes[nodeIndex];
        var oldData = targetNode.Data;
        if (oldData != null && oldData.Equals(newData))
        {
            return false;
        }

        if (canReplace != null && !canReplace(oldData, newData))
        {
            return false;
        }

        targetNode.Data = newData;
        _nodes[nodeIndex] = targetNode;

        for (var i = way.Length - 1; i >= 0; i--)
        {
            var parentIndex = parentIndices[i];
            if (!TryMerge(parentIndex))
            {
                break;
            }
        }

        return true;
    }

    public void SetNodeData(int index, T data)
    {
        var linearOctreeNode = _nodes[index];
        linearOctreeNode.Data = data;
        _nodes[index] = linearOctreeNode;
    }


    public int Split(int index)
    {
        var parent = _nodes[index];
        if (!parent.IsLeaf) throw new ArgumentException("Node is already splitted", nameof(index));

        int startChildIndex;
        if (_freeClusters.TryPop(out int recycledIndex))
        {
            startChildIndex = recycledIndex;
            for (var i = 0; i < 8; i++)
            {
                // ПЕРЕЗАПИСУЄМО старі дані за існуючими індексами
                _nodes[startChildIndex + i] = new LinearOctreeNode<T>(parent.Data);
            }
        }
        else
        {
            // Якщо вільних немає, виділяємо нове місце в кінці
            startChildIndex = _nodes.Count;
            for (var i = 0; i < 8; i++)
            {
                // Додаємо нові елементи
                _nodes.Add(new LinearOctreeNode<T>(parent.Data));
            }
        }

        parent.ChildrenStartIndex = startChildIndex;
        _nodes[index] = parent;

        return startChildIndex;
    }

    public bool TryMerge(int index)
    {
        var parent = _nodes[index];
        if (parent.IsLeaf) return false;

        int firstChildIdx = parent.ChildrenStartIndex;
        var firstChild = _nodes[firstChildIdx];

        if (!firstChild.IsLeaf) return false;

        for (int i = 1; i < 8; i++)
        {
            var child = _nodes[firstChildIdx + i];

            if (!child.IsLeaf || !EqualityComparer<T>.Default.Equals(child.Data, firstChild.Data))
            {
                return false;
            }
        }

        parent.Data = firstChild.Data;
        parent.MarkAsLeaf();
        _nodes[index] = parent;

        _freeClusters.Push(firstChildIdx);
        // TODO: Додати індекси дітей у список вільних комірок для повторного використання
        return true;
    }

    public bool Merge(int index)
    {
        var parent = _nodes[index];
        if (parent.IsLeaf) return false;

        _freeClusters.Push(parent.ChildrenStartIndex);
        parent.MarkAsLeaf();
        _nodes[index] = parent;
        return true;
    }
    // public void Merge(int index)
    // {
    //     if (_nodes[index].IsLeaf) return;
    //     _nodes[index].MarkAsLeaf();
    // }

    public void Clear()
    {
        _nodes.Clear();
    }

    public void Pack()
    {
        // Якщо сміття немає, нічого не робимо
        if (_freeClusters.Count == 0) return;

        // Виділяємо пам'ять тільки під реальну кількість живих нодів
        var packedNodes = new List<LinearOctreeNode<T>>(_nodes.Count - _freeClusters.Count * 8);
        var queue = new Queue<(int oldIndex, int newIndex)>();

        // 1. Переносимо корінь
        packedNodes.Add(_nodes[RootIndex]);
        queue.Enqueue((RootIndex, 0)); // (Індекс у старому масиві, Індекс у новому масиві)

        // 2. Обходимо всі живі ноди
        while (queue.Count > 0)
        {
            var (oldIdx, newIdx) = queue.Dequeue();
            var oldNode = _nodes[oldIdx];

            if (!oldNode.IsLeaf)
            {
                // Отримуємо індекс, куди зараз будуть записані діти
                int newChildrenStart = packedNodes.Count;

                // Оновлюємо вказівник на дітей у батька (в НОВОМУ масиві)
                var packedNode = packedNodes[newIdx];
                packedNode.ChildrenStartIndex = newChildrenStart;
                packedNodes[newIdx] = packedNode;

                // Копіюємо всіх 8 дітей
                for (int i = 0; i < 8; i++)
                {
                    int oldChildIdx = oldNode.ChildrenStartIndex + i;
                    int newChildIdx = packedNodes.Count;

                    packedNodes.Add(_nodes[oldChildIdx]);

                    // Якщо дитина теж має своїх дітей, додаємо в чергу для обробки
                    if (!_nodes[oldChildIdx].IsLeaf)
                    {
                        queue.Enqueue((oldChildIdx, newChildIdx));
                    }
                }
            }
        }

        // 3. Замінюємо старі дані на чисті
        _nodes.Clear();
        _nodes.AddRange(packedNodes);
        _freeClusters.Clear(); // Тепер сміття немає, стек пустий
    }

    public OctreeMemento<T> Save()
    {
        Pack();
        // Зберігаємо масив як є. Тобі доведеться додати збереження _freeClusters у Memento!
        return new OctreeMemento<T>(_nodes.ToArray());
    }

    public void Restore(OctreeMemento<T> memento)
    {
        _nodes.Clear();
        _nodes.AddRange(memento.Nodes);

        _freeClusters.Clear();
    }
}