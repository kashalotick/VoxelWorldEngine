using VoxelModule.Content.Commands;
using VoxelModule.Content.Generators;
using VoxelModule.Engine.Octree;
using Chunk = VoxelModule.Engine.Chunks.Chunk;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine.Builders;

public class ChunkBuilder
{
    private readonly int _seed;
    private SurfaceGenerator _surfaceGenerator;
    private VegetationGenerator _vegetationGenerator;
    private TreeCommandFactory _treeFactory;

    private const int VegetationBorderRadius = 3;
    
    public ChunkBuilder(int seed)
    {
        _seed = seed;
        _surfaceGenerator = new SurfaceGenerator(_seed, Vector3Int.Zero);
        _vegetationGenerator = new VegetationGenerator(_surfaceGenerator, Chunk.ChunkSize, VegetationBorderRadius, _seed);
        _treeFactory = new TreeCommandFactory();
    }

    public Chunk Build(Vector3Int position)
    {
        var chunk = new Chunk(position);

        _surfaceGenerator.Offset = chunk.GlobalPosition;
        var octree = new VoxelOctree();
        octree.Build(_surfaceGenerator);
        chunk.Octree = octree;

        var treePoints = _vegetationGenerator.GetTreeSpawnPoints(position);
        foreach (var point in treePoints)
        {
            PlaceTree(chunk, point);
        }

        return chunk;
    }

    public void PlaceTree(Chunk chunk, Vector3Int position)
    {
        var command = _treeFactory.GetCommand(chunk, position, new TreeArgs(_seed));
        command.Execute();
    }
}