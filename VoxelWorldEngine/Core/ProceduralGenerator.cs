using System.Numerics;
using DotnetNoise;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

public class ProceduralGenerator : IGenerator
{
    private Vector3Int _offset;
    private FastNoise _noise;
    private FastNoise _noise2;


    private (Vector2Int min, Vector2Int max) _cacheVector;
    private (float min, float max) _cacheValue;

    public ProceduralGenerator(int seed, Vector3Int offset)
    {
        _offset = offset;
        _noise = new FastNoise(seed);
        _noise2 = new FastNoise(seed)
        {
            Frequency = 0.03f,
            Lacunarity = 3f,
            Gain = 0.75f,
        };
    }

    public Voxel Approximate(Vector3Int min, Vector3Int max)
    {
        min += _offset;
        max += _offset;

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
        min += _offset;
        max += _offset;

        var height = Evaluate(min, max);

        if (max.Y <= height.min)
            return true;

        if (min.Y >= height.max)
            return true;

        return false;
    }

    private (float min, float max) Evaluate(Vector3Int min, Vector3Int max)
    {
        var min2 = new Vector2Int(min.X, min.Z);
        var max2 = new Vector2Int(max.X, max.Z);
        if (_cacheVector == (min2, max2))
        {
            return _cacheValue;
        }

        _cacheVector = (min2, max2);
        var height = _noise.GetNoiseMinMax(min2, max2);
        var subHeight = _noise2.GetNoiseMinMax(min2 * 3, max2 * 3);

        var baseHeight = _noise.GetNoiseMinMax((Vector2)min2 * 0.1f, (Vector2)max2 * 0.1f);


        var newMin = height.min + subHeight.min * 0.05f;
        var newMax = height.max + subHeight.max * 0.05f;

        newMin += baseHeight.min * 2;
        newMax += baseHeight.max * 2;


        _cacheValue = (ModifyHeight(newMin), ModifyHeight(newMax));
        return _cacheValue;
    }

    private float ModifyHeight(float value)
    {
        var baseHeight = 0;
        var amplitude = 25;

        var result = baseHeight + value * amplitude;
        return result;
    }
}