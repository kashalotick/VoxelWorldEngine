using VoxelModule.Core.Serialization;
using VoxelModule.Engine.Builders;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine.Chunks;

public class ChunkFactory
{
    private IChunkMementoRepository _repository;
    private readonly IMeshBuilder _meshBuilder;
    private readonly ChunkBuilder _chunkBuilder;

    public ChunkFactory(IChunkMementoRepository repository, int seed)
    {
        _repository = repository;
        _meshBuilder = new MeshBuilder();
        _chunkBuilder = new ChunkBuilder(seed);
    }


    public Chunk GetChunk(Vector3Int chunkPosition)
    {
        Chunk chunk;
        var memento = _repository.Load(chunkPosition);

        if (memento is null)
        {
            chunk = _chunkBuilder.Build(chunkPosition);
        }
        else
        {
            chunk = new Chunk(chunkPosition);
            chunk.Restore(memento);
        }

        chunk.ChunkMesh = _meshBuilder.Build(chunk.Octree);

        return chunk;
    }
}