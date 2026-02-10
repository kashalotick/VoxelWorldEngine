using System.Runtime.InteropServices.ComTypes;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.LinearOctree;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public class OctreeNode<T> : IOctreeNode<T>
{
    private LinearOctree<T> _linearOctree;
    private int _linearIndex;
    
    public int MaxDepth { get; set; }
    public int Depth { get; set;}
    public int Size => 1 << (MaxDepth - Depth);

    public Vector3Int Min { get; set; }
    public Vector3Int Max => Min + Vector3Int.One * (Size - 1);


    public IOctreeNode<T>[] Children
    {
        get
        {
            if (IsLeaf) return null;
            
            var childSize = Size >> 1;
            
            var children = new OctreeNode<T>[8];
            for (int i = 0; i < 8; i++)
            {
                var corner = OctreeMath.GetOctantCorner(i, childSize);
                var childLinearIndex = _linearOctree.GetNode(_linearIndex).GetChildIndex(i);
                var child = new OctreeNode<T>(_linearOctree, _linearIndex + i)
                {
                    _linearOctree = _linearOctree,
                    _linearIndex = childLinearIndex,
                    MaxDepth = MaxDepth,
                    Depth = Depth - 1,
                    Min = corner,
                };
                children[i] = child;
            }


            return children;
        }
    }


    public OctreeNode(LinearOctree<T> linearOctree, int linearIndex)
    {
        _linearOctree = linearOctree;
        _linearIndex = linearIndex;
    }

    public IEnumerable<T> Query(Vector3Int min, Vector3Int max)
    {
        throw new NotImplementedException();
    }

    public T Data
    {
        get => _linearOctree.GetNode(_linearIndex).Data;
        set => _linearOctree.SetNodeData(_linearIndex, value);
    }

    public bool IsLeaf => _linearOctree.GetNode(_linearIndex).IsLeaf;

    public void Split()
    {
        _linearOctree.Split(_linearIndex);
    }

    public bool Merge()
    {
        return _linearOctree.Merge(_linearIndex);
    }

    public bool TryMerge()
    {
        return _linearOctree.TryMerge(_linearIndex);
    }
}