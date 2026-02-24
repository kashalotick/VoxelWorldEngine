using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core.Generators;

public struct GenerationContext
{
    public Vector3Int Min;
    public Vector3Int Max;
    public long Seed;
}