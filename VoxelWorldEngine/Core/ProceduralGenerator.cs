using DotnetNoise;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

public class ProceduralGenerator : IGenerator
{
    private Vector3Int _offset;
    private FastNoise _noise;

    public ProceduralGenerator(int seed, Vector3Int offset)
    {
        _offset = offset;
        _noise = new FastNoise(seed);
    }

    public Voxel Approximate(Vector3Int min, Vector3Int max)
    {
        Voxel mixed = new Voxel(2);
        Voxel solid = new Voxel(1);
        Voxel air = new Voxel(0);

        var height = Evaluate(min, max);

        if (max.Y <= height.min)
            return solid;

        if (min.Y >= height.max)
            return air;

        return mixed; // throw ??
    }

    public bool IsUniform(Vector3Int min, Vector3Int max)
    {
        var height = Evaluate(min, max);

        if (max.Y <= height.min)
            return true;

        if (min.Y >= height.max)
            return true;

        return false;
    }

    private (float min, float max) Evaluate(Vector3Int min, Vector3Int max)
    {
        min += _offset;
        max += _offset;

        var height = _noise.GetNoiseMinMax(new Vector2Int(min.X, min.Z), new Vector2Int(max.X, max.Z));
        return (ModifyHeight(height.min), ModifyHeight(height.max));
    }

    private float ModifyHeight(float value)
    {
        var baseHeight = 0;
        var amplitude = 25;

        var result = baseHeight + value * amplitude;
        return result;
    }
}