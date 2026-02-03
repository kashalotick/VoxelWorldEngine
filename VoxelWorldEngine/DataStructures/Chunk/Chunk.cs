namespace VoxelWorldEngine.DataStructures.Chunk;

// TODO: implement logic
public class Chunk
{
    
    public Vector3Int.Vector3Int Position;
    public LinearOctree.LinearOctree Octree;
    public List<Mesh.Mesh> Meshes;

    public Chunk(Vector3Int.Vector3Int position)
    {
        Position = position;
    }

    public void SetOctree(LinearOctree.LinearOctree octree)
    {
        Octree = octree;
    }
    
    public void SetMeshes(List<Mesh.Mesh> meshes)
    {
        Meshes = meshes;
    }
}