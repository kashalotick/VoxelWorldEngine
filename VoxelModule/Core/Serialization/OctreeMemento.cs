using VoxelModule.Core.Trees.LinearImplementation;

namespace VoxelModule.Core.Serialization;

public record OctreeMemento<T>
{
    public LinearOctreeNode<T>[] Nodes { get; }

    public OctreeMemento(LinearOctreeNode<T>[] nodes)
    {
        Nodes = nodes;
    }
}