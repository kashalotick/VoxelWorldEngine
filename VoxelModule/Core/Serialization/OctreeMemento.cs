using VoxelModule.DataStructures.Collections.Trees.LinearImplementation;

namespace VoxelModule.Core.Serialization;

public record OctreeMemento<T>
{
    public LinearOctreeNode<T>[] Nodes { get; }

    public OctreeMemento(LinearOctreeNode<T>[] nodes)
    {
        Nodes = nodes;
    }
}