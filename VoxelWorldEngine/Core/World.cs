using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core;

public class World
{
    public event Action<Chunk> ChunkAdded;

    public event Action<Chunk> ChunkUpdated;
    public event Action<Vector3Int> ChunkRemoved;
    
    public World(int seed)
    {
        Seed = seed;
    }

    public int Seed { get; }
    private Dictionary<Vector3Int, Chunk> _chunks = new();
    public IReadOnlyDictionary<Vector3Int, Chunk>  Chunks => _chunks;


    public void AddChunk(Chunk chunk)
    {
        _chunks[chunk.Position] = chunk;
        ChunkAdded?.Invoke(chunk);

    }
    public void UpdateChunk(Chunk chunk)
    {
        _chunks[chunk.Position] = chunk;
        ChunkUpdated?.Invoke(chunk);
    }

    public void RemoveChunk(Vector3Int chunkPosition)
    {
        _chunks.Remove(chunkPosition);
        ChunkRemoved?.Invoke(chunkPosition);
    }

}