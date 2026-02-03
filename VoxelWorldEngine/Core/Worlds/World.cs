using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.DataStructures.Voxel;

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