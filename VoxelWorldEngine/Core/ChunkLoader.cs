using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core;

public class ChunkLoader(World world)
{
    private World _world = world;

    // make chunk pipeline ??
    // strategy: generate, restore (deserialize), combine (apply changes to generated)
    public Chunk Get(Vector3Int chunkPosition)
    {
        var builder = new ChunkBuilder(_world.Seed);
        var chunk = builder.Build(chunkPosition);
        
        var meshBuilder = new MeshBuilder(chunk.Octree);
        var mesh = meshBuilder.Build();
        
        chunk.Mesh = mesh;

        return chunk;
    }
    
}