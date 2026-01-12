using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Chunk;

/// <summary>
///     Provides mathematical operations for converting between global, chunk, and local positions
///     within the context of voxel-based chunk systems.
/// </summary>
public static class ChunkMath
{
    private static int ChunkSize => LinearOctree.LinearOctree.Size;
    
    /// <summary>
    ///     Convert the global position into the position of 0's chunk corner (0,0,0 local).
    /// </summary>
    /// <param name="position">Position of chunk.</param>
    /// <returns>Global position of 0's chunk corner.</returns>
    public static Vector3Int.Vector3Int ChunkToGlobal(Vector3Int.Vector3Int position)
    {
        return position * ChunkSize;
    }
    /// <summary>
    ///     Convert the global position into the position of chunk.
    /// </summary>
    /// <param name="position">Global position.</param>
    /// <returns>Position of chunk.</returns>
    public static Vector3Int.Vector3Int GlobalToChunk(Vector3Int.Vector3Int position)
    {
        return new Vector3Int.Vector3Int(
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
    public static Vector3Int.Vector3Int GlobalToLocal(Vector3Int.Vector3Int position)
    {
        return new Vector3Int.Vector3Int(
            MathHelper.Mod(position.X, ChunkSize),
            MathHelper.Mod(position.Y, ChunkSize),
            MathHelper.Mod(position.Z, ChunkSize)
        );
    }
}