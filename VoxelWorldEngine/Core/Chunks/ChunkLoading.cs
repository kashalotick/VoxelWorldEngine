using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Chunks;

public class ChunkLoading : AbstractChunk
{
    public ChunkLoading(Vector3Int position) : base(position)
    {
    }

    public Chunk Load()
    {
        return new Chunk(Position);
    }

    public void Unload()
    {
        throw new NotImplementedException();
    }

    
    // deserialize
    public Chunk Restore()
    {
        throw new NotImplementedException();
    }

    // new clear
    public Chunk Generate()
    {
        throw new NotImplementedException();
    }

}