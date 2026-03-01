using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core;

public class ChunkLoader
{
    private readonly World _world;
    private readonly ChunkBuilder _chunkBuilder;
    private readonly MeshBuilder _meshBuilder;
    
    public ChunkLoader(World world)
    {
        _world = world;
        _chunkBuilder = new ChunkBuilder(_world.Seed);
        _meshBuilder = new MeshBuilder();
    }
    
    // make chunk pipeline ??
    // strategy: generate, restore (deserialize), combine (apply changes to generated)
    public Chunk Get(Vector3Int chunkPosition)
    {
        var chunk = _chunkBuilder.Build(chunkPosition);
        var mesh = _meshBuilder.Build(chunk.Octree);
        
        chunk.Mesh = mesh;

        return chunk;
    }
    
}