using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core.Builders;

public class ChunkBuilder
{
    private readonly int _seed;
    private ProceduralGenerator _generator;

    public ChunkBuilder(int seed)
    {
        _seed = seed;
        _generator = new ProceduralGenerator(_seed, Vector3Int.Zero);
    }

    public Chunk Build(Vector3Int position)
    {
        var chunk = new Chunk(position);

        _generator.Offset = chunk.GlobalPosition;
        var octree = new VoxelOctree();
        octree.Build(_generator);

        chunk.Octree = octree;

        return chunk;
    }
}