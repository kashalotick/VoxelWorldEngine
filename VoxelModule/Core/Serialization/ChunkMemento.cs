using VoxelModule.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.DataStructures.Special.Structures.Voxels;
using VoxelModule.DataStructures.Common.Collections.Trees;

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
    public ChunkMemento(Vector3Int position, OctreeMemento<Voxel> octree)
    {
        Position = position;
        Nodes = octree.Nodes;
    }
}

