using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core.Chunks;

public partial class Chunk{
    public static int ChunkSize => Constants.ChunkSize;

    public static Vector3Int ChunkToGlobal(Vector3Int position)
    {
        return position * ChunkSize;
    }

    public static Vector3Int GlobalToChunk(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.FloorDiv(position.X, ChunkSize),
            MathHelper.FloorDiv(position.Y, ChunkSize),
            MathHelper.FloorDiv(position.Z, ChunkSize)
        );
    }

    public static Vector3Int GlobalToLocal(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.Mod(position.X, ChunkSize),
            MathHelper.Mod(position.Y, ChunkSize),
            MathHelper.Mod(position.Z, ChunkSize)
        );
    }

}