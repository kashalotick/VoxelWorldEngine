using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Serialization;

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

