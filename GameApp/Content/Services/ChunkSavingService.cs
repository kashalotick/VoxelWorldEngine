using VoxelModule.Core.Serialization;
using Chunk = VoxelModule.Engine.Chunks.Chunk;

namespace GameApp.Content.Services;

public class ChunkSavingService
{
    private readonly IChunkMementoRepository _chunkRepository;

    public ChunkSavingService(IChunkMementoRepository chunkRepository)
    {
        _chunkRepository = chunkRepository;
    }

    public void OnRemoveChunk(Chunk chunk)
    {
        if (chunk.IsDirty)
        {
            _chunkRepository.Save(chunk.Save());
        }
    }
}
