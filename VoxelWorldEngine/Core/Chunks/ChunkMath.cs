using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Chunks;

public static class ChunkMath
{
    public static Vector3Int GetChunkPosition(Vector3Int voxelPosition)
    {
        var x = voxelPosition.X >> Chunk.ChunkSizeBits;
        var y = voxelPosition.Y >> Chunk.ChunkSizeBits;
        var z = voxelPosition.Z >> Chunk.ChunkSizeBits;
        return new Vector3Int(x, y, z);
    }
    // public static Vector3Int GetChunkPosition(int x, int y, int z)
    // {
    //     return new Vector3Int(x >> Chunk.ChunkSizeBits, y >> Chunk.ChunkSizeBits, z >> Chunk.ChunkSizeBits);
    // }
}