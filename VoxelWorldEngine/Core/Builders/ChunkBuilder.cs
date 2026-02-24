using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core.Builders;

public class ChunkBuilder
{
    private readonly int _seed;
    
    public ChunkBuilder(int seed)
    {
        _seed = seed;
    }

    public Chunk Build(Vector3Int position)
    {

        IGenerator generator = new ProceduralGenerator(_seed, position);
        var octree = new VoxelOctree();
        octree.Build(generator);

        var meshBuilder = new MeshBuilder(octree);
        var mesh = meshBuilder.Build();

        var chunk = new Chunk(position, octree, mesh);
        
        return chunk;
    }
}