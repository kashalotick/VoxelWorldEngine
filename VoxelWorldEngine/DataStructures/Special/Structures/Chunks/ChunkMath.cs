using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

/// <summary>
///     Provides mathematical operations for converting between global, chunk, and local positions
///     within the context of voxel-based chunk systems.
/// </summary>
public static class ChunkMath
{
    private static int ChunkSize => Constants.ChunkSize;

    /// <summary>
    ///     Convert the global position into the position of 0's chunk corner (0,0,0 local).
    /// </summary>
    /// <param name="position">Position of chunk.</param>
    /// <returns>Global position of 0's chunk corner.</returns>
    public static Vector3Int ChunkToGlobal(Vector3Int position)
    {
        return position * ChunkSize;
    }

    /// <summary>
    ///     Convert the global position into the position of chunk.
    /// </summary>
    /// <param name="position">Global position.</param>
    /// <returns>Position of chunk.</returns>
    public static Vector3Int GlobalToChunk(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.FloorDiv(position.X, ChunkSize),
            MathHelper.FloorDiv(position.Y, ChunkSize),
            MathHelper.FloorDiv(position.Z, ChunkSize)
        );
    }

    /// <summary>
    ///     Convert the global position into the local position in chunk.
    /// </summary>
    /// <param name="position">Global position.</param>
    /// <returns>Local position.</returns>
    public static Vector3Int GlobalToLocal(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.Mod(position.X, ChunkSize),
            MathHelper.Mod(position.Y, ChunkSize),
            MathHelper.Mod(position.Z, ChunkSize)
        );
    }
}