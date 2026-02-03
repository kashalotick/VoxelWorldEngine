using DotnetNoise;
using VoxelWorldEngine.Core.Generators;
using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Core.Builders;

public class ChunkBuilder
{
    public Chunk Build(Vector3Int chunkPosition)
    {
        var chunk = new Chunk(chunkPosition);
        var fastnoise = new FastNoise(1234);
        var heightMapGenerator = new HeightMapGenerator(fastnoise);
        var densityGenerator = new DensityGenerator(heightMapGenerator);
        var octreeBuilder = new OctreeBuilder(densityGenerator);
        var octree = octreeBuilder.Build(chunk);
        
        chunk.SetOctree(octree);
        
        var meshBuilder = new MeshBuilder();
        meshBuilder.Build(chunk.Octree);
        
        chunk.SetMeshes(meshBuilder.BuildMeshList(octree));
        
        return chunk;
    }
}