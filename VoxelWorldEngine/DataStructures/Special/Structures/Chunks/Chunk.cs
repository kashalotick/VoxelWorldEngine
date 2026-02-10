using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

// TODO: implement logic
public class Chunk
{
    public List<Mesh> Meshes;
    public VoxelOctree Octree;

    public Vector3Int Position;

    public Chunk(Vector3Int position)
    {
        Position = position;
    }

    public void SetOctree(VoxelOctree octree)
    {
        Octree = octree;
    }

    public void SetMeshes(List<Mesh> meshes)
    {
        Meshes = meshes;
    }
}