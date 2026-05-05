using VoxelModule.Core.Trees.LinearImplementation;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Serialization;

public record ChunkMemento
{
    public Vector3Int Position { get; }
    public LinearOctreeNode<Voxel>[] Nodes { get; }

    
    public ChunkMemento(Vector3Int position, LinearOctreeNode<Voxel>[] nodes)
    {
        Position = position;
        Nodes = nodes;
    }
}

