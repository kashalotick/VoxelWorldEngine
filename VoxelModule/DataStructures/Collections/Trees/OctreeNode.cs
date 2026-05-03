using VoxelModule.DataStructures.Collections.Trees.LinearImplementation;
using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.Utils;

namespace VoxelModule.DataStructures.Collections.Trees;

public partial class Octree<T>
{
    internal readonly struct OctreeNode : IOctreeNodeReadonly<T>
    {
        private readonly LinearOctree<T> _linearOctree;
        private readonly int LinearIndex;
        private readonly int? ParentLinearIndex;

        public int Depth { get; init; }
        public int Size => 1 << (_linearOctree.MaxDepth - Depth);

        public Vector3Int MinIndex { get; init; }
        public Vector3Int MaxIndex => MinIndex + Vector3Int.One * (Size - 1);
        public Vector3Int MinAABB => MinIndex;
        public Vector3Int MaxAABB => MaxIndex + Vector3Int.One;


        public T Data
        {
            get => _linearOctree.GetNode(LinearIndex).Data;
            set => _linearOctree.SetNodeData(LinearIndex, value);
        }

        public bool IsLeaf => _linearOctree.GetNode(LinearIndex).IsLeaf;

        public OctreeNode(LinearOctree<T> linearOctree, int linearLinearIndex, int? parentLinearIndex = null)
        {
            _linearOctree = linearOctree;
            LinearIndex = linearLinearIndex;
            ParentLinearIndex = parentLinearIndex;
        }

        public OctreeNode GetChild(int octant)
        {
            var childLinearIndex = _linearOctree.GetNode(LinearIndex).GetChildIndex(octant);
            var corner = MinIndex + OctreeMath.GetOctantCorner(octant, Size);
            var child = new OctreeNode(_linearOctree, childLinearIndex, LinearIndex)
            {
                Depth = Depth + 1,
                MinIndex = corner,
            };
            return child;
        }

        public IOctreeNodeReadonly<T>? GetNeighbor(Vector3Int direction)
        {
            var neighborMin = MinIndex + direction * Size;

            var treeSize = _linearOctree.Size;
            if (neighborMin.X < 0
                || neighborMin.Y < 0
                || neighborMin.Z < 0
                || neighborMin.X >= treeSize
                || neighborMin.Y >= treeSize
                || neighborMin.Z >= treeSize)
            {
                return null;
            }

            // FindWayTo дає шлях на максимальну глибину, нам треба тільки Depth кроків
            Span<int> way = stackalloc int[_linearOctree.MaxDepth];
            OctreeMath.FindWayTo(neighborMin, treeSize, way);

            var nodeIndex = _linearOctree.RootIndex;
            var nodeMin = Vector3Int.Zero;
            var nodeSize = treeSize;

            for (int i = 0; i < Depth; i++)
            {
                var node = _linearOctree.GetNode(nodeIndex);
                if (node.IsLeaf)
                {
                    // великий leaf покриває нашу точку — повертаємо його
                    return new OctreeNode(_linearOctree, nodeIndex)
                    {
                        Depth = i,
                        MinIndex = nodeMin,
                    };
                }

                var octant = way[i];
                nodeIndex = node.GetChildIndex(octant);
                nodeMin += OctreeMath.GetOctantCorner(octant, nodeSize);
                nodeSize >>= 1;
            }

            return new OctreeNode(_linearOctree, nodeIndex)
            {
                Depth = Depth,
                MinIndex = nodeMin,
            };
        }

        public IEnumerable<T> Query(Vector3Int min, Vector3Int max)
        {
            if (!Intersects(min, max))
            {
                return Enumerable.Empty<T>();
            }

            var cMin = ClampHelper.ClampVector3(min, MinIndex, MaxIndex);
            var cMax = ClampHelper.ClampVector3(max, MinIndex, MaxIndex);

            long countX = (long)cMax.X - cMin.X + 1;
            long countY = (long)cMax.Y - cMin.Y + 1;
            long countZ = (long)cMax.Z - cMin.Z + 1;

            long estimatedVolume = countX * countY * countZ;
            int finalCapacity = (int)Math.Min(estimatedVolume, _linearOctree.Count);
            finalCapacity = Math.Clamp(finalCapacity, 0, 100000);

            var result = new List<T>(finalCapacity);

            QueryRecursive(result, min, max);

            return result;
        }


        internal void QueryRecursive(List<T> queryResult, Vector3Int min, Vector3Int max)
        {
            if (!Intersects(min, max)) return;

            if (IsLeaf)
            {
                queryResult.Add(Data);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    var node = GetChild(i);
                    node.QueryRecursive(queryResult, min, max);
                }
            }
        }

        public void Accept(IOctreeVisitor<T> visitor)
        {
            if (IsLeaf)
            {
                visitor.Visit(this);
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                GetChild(i).Accept(visitor);
            }
        }

        public bool Intersects(Vector3Int min, Vector3Int max)
        {
            return MinIndex.X <= max.X
                   && MaxIndex.X >= min.X
                   && MinIndex.Y <= max.Y
                   && MaxIndex.Y >= min.Y
                   && MinIndex.Z <= max.Z
                   && MaxIndex.Z >= min.Z;
        }

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