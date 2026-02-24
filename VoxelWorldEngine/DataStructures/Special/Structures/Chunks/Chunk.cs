using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

// TODO: implement logic
public class Chunk
{
    public MeshData Mesh { get; set;}
    public VoxelOctree Octree { get; set;}
    public Vector3Int Position { get; set;}
    

    public Chunk(Vector3Int position, VoxelOctree octree, MeshData mesh)
    {
        Position = position;
        Octree = octree;
        Mesh = mesh;
    }
}