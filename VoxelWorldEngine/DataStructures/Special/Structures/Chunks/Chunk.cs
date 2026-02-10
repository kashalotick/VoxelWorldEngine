using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Mesh;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

// TODO: implement logic
public class Chunk
{
    public List<Mesh> Meshes;
    public LinearOctree.LinearOctree Octree;

    public Vector3Int Position;

    public Chunk(Vector3Int position)
    {
        Position = position;
    }

    public void SetOctree(LinearOctree.LinearOctree octree)
    {
        Octree = octree;
    }

    public void SetMeshes(List<Mesh> meshes)
    {
        Meshes = meshes;
    }
}