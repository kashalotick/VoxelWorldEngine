using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace VoxelWorldEngine.Core;

public class ChunkLoader
{
    private readonly VoxelWorld _voxelWorld;
    private readonly ChunkBuilder _chunkBuilder;
    private readonly MeshBuilder _meshBuilder;
    
    public ChunkLoader(VoxelWorld voxelWorld)
    {
        _voxelWorld = voxelWorld;
        _chunkBuilder = new ChunkBuilder(_voxelWorld.Seed);
        _meshBuilder = new MeshBuilder();
    }
    
    // make chunk pipeline ??
    // strategy: generate, restore (deserialize), combine (apply changes to generated)
    public Chunk BuildChunk(Vector3Int chunkPosition)
    {
        var chunk = _chunkBuilder.Build(chunkPosition);
        var mesh = BuildMesh(chunk);
        
        chunk.Mesh = mesh;

        return chunk;
    }
    
    public MeshData BuildMesh(Chunk chunk)
    {
        var mesh = _meshBuilder.Build(chunk.Octree);

        return mesh;
    }
    
        
    public Chunk UpdateMesh(Chunk chunk)
    {
        var mesh = _meshBuilder.Build(chunk.Octree);
        chunk.Mesh =  mesh;
        return chunk;
    }
    
}