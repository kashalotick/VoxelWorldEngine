using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace VoxelWorldEngine.Core.Worlds;

public class World
{
    
    
    
    public Seed Seed;
    private ChunkManager _chunkManager;

    public World(Seed seed)
    {
        Seed = seed;
        _chunkManager = new ChunkManager();
    }

    public Voxel GetVoxel(Vector3Int position)
    {
        throw new NotImplementedException();
    }

    public void SetVoxel(Vector3Int position, Voxel voxel)
    {
        throw new NotImplementedException();
    }

    public bool Raycast(Vector3Int start, Vector3Int direction, out Vector3Int hitPosition)
    {
        // make out RaycastHit ??
        throw new NotImplementedException();
    }

    public void Update(double deltaTime, Observer observer)
    {
        _chunkManager.Update(deltaTime, observer);
    }
    
    
}