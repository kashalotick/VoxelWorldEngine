using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core;

public interface IChunkDirtySink
{
    void MarkDirty(Vector3Int chunkCoords);
}

