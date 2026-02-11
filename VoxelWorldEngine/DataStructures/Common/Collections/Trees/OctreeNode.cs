using System.Runtime.InteropServices.ComTypes;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.LinearOctree;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public partial class Octree<T>
{
    protected readonly struct OctreeNode
    {
        private LinearOctree<T> _linearOctree { get; init; }
        private int LinearIndex { get; init; }

        public int Depth { get; init; }
        public int Size => 1 << (MaxDepth - Depth);

        public Vector3Int Min { get; init; }
        public Vector3Int Max => Min + Vector3Int.One * (Size - 1);

        public T Data
        {
            get => _linearOctree.GetNode(LinearIndex).Data;
            set => _linearOctree.SetNodeData(LinearIndex, value);
        }

        public OctreeNode(LinearOctree<T> linearOctree, int linearIndex, int maxDepth, int depth, Vector3Int min)
        {
            _linearOctree = linearOctree;
            LinearIndex = linearIndex;
            Depth = depth;
            Min = min;
        }

        public OctreeNode GetChild(int octant)
        {
            var childLinearIndex = _linearOctree.GetNode(LinearIndex).GetChildIndex(octant);
            var corner = Min + OctreeMath.GetOctantCorner(octant, Size >> 1);
            var child = new OctreeNode(_linearOctree, childLinearIndex)
            {
                _linearOctree = _linearOctree,
                LinearIndex = childLinearIndex,
                Depth = Depth + 1,
                Min = corner,
            };
            return child;
        }


        public OctreeNode(LinearOctree<T> linearOctree, int linearLinearIndex)
        {
            _linearOctree = linearOctree;
            LinearIndex = linearLinearIndex;
        }

        public IEnumerable<T> Query(Vector3Int min, Vector3Int max)
        {
            throw new NotImplementedException();
        }

        public bool IsLeaf => _linearOctree.GetNode(LinearIndex).IsLeaf;

        public void Split()
        {
            _linearOctree.Split(LinearIndex);
        }

        public bool Merge()
        {
            return _linearOctree.Merge(LinearIndex);
        }

        public bool TryMerge()
        {
            return _linearOctree.TryMerge(LinearIndex);
        }
    }
}