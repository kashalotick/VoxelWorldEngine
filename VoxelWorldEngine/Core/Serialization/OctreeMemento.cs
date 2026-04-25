using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;

namespace VoxelWorldEngine.Core.Serialization;

public record OctreeMemento<T>
{
    public LinearOctreeNode<T>[] Nodes { get; }

    public OctreeMemento(LinearOctreeNode<T>[] nodes)
    {
        Nodes = nodes;
    }
}