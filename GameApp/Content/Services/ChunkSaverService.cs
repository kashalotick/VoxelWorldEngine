using VoxelModule.Core.Chunks;
using VoxelModule.Core.Serialization;

namespace GameApp.Content.Services;

public class ChunkSaverService
{
    private readonly IChunkMementoRepository _chunkRepository;

    public ChunkSaverService(IChunkMementoRepository chunkRepository)
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
