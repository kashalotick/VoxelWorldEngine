using System.Runtime.InteropServices.ComTypes;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public class OctreeNode<T> : IOctreeNode<T>
    where T : struct
{
    private LinearOctree<T> _linearOctree;
    private int _linearIndex;

    public int MaxDepth { get; set; }
    public int Depth;
    public int Size => 1 << (MaxDepth - Depth);

    public Vector3Int Min { get; set; }
    public Vector3Int Max => Min + Vector3Int.One * (Size - 1);
//   


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