using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Chunks;

public class AbstractChunk
{
    public const int ChunkSizeBits = 4;
    public const int ChunkSize = 2 << ChunkSizeBits;

    public Vector3Int Position;
    public bool IsDirty { get; protected set; }

    
    public AbstractChunk(Vector3Int position)
    {
        IsDirty = false;
        Position = position;
    }
}