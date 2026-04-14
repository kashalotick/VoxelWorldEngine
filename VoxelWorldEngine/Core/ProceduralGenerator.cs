using System.Numerics;
using DotnetNoise;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core;

public class ProceduralGenerator : IGenerator
{
    public Vector3Int Offset;
    private FastNoise _noise;
    private FastNoise _noise2;

    private const int DirtDepth = 4;
    private const int GrassDepth = 1; // Тепер використовується

    private (Vector2Int min, Vector2Int max) _cacheVector;
    private (float min, float max) _cacheValue;

    public ProceduralGenerator(int seed, Vector3Int offset)
    {
        Offset = offset;
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
        min += Offset;
        max += Offset;

        var height = Evaluate(min, max);

        // 1. Повністю під dirt-зоною — гарантовано камінь
        if (max.Y <= height.min - DirtDepth)
            return new Voxel(BlockId.Stone);

        // 2. Повністю над поверхнею — гарантовано повітря
        if (min.Y > height.max)
            return new Voxel(BlockId.Air);

        // Визначаємо тип блоку для змішаних зон (або листкових нод 1x1x1)
        var centerY = (min.Y + max.Y) / 2f;
        var centerHeight = (height.min + height.max) / 2f;

        // Повітря над поверхнею
        if (centerY > centerHeight)
            return new Voxel(BlockId.Air);

        // Трава — самий верхній шар (товщиною GrassDepth)
        if (centerY > centerHeight - GrassDepth)
            return new Voxel(BlockId.Grass);

        // Земля — під травою до певної глибини
        if (centerY > centerHeight - DirtDepth)
            return new Voxel(BlockId.Dirt);

        // Камінь — все, що глибше за DirtDepth (виправляє баг з землею внизу)
        return new Voxel(BlockId.Stone);
    }

    public bool IsUniform(Vector3Int min, Vector3Int max)
    {
        min += Offset;
        max += Offset;

        var height = Evaluate(min, max);

        // Регіон однорідний, якщо він повністю в зоні каменю або повністю в повітрі
        if (max.Y <= height.min - DirtDepth)
            return true;

        if (min.Y > height.max)
            return true;

        return false;
    }

    private (float min, float max) Evaluate(Vector3Int min, Vector3Int max)
    {
        var min2 = new Vector2Int(min.X, min.Z);
        var max2 = new Vector2Int(max.X, max.Z);

        const int absoluteMin = -32;
        const int absoluteMax = 156;

        if (max.Y <= absoluteMin)
            return (absoluteMin, absoluteMin);

        if (min.Y >= absoluteMax)
            return (absoluteMax, absoluteMax);

        if (_cacheVector == (min2, max2))
            return _cacheValue;

        _cacheVector = (min2, max2);

        var height     = _noise.GetNoiseMinMax(min2, max2);
        var subHeight  = _noise2.GetNoiseMinMax(min2 * 3, max2 * 3);
        var baseHeight = _noise.GetNoiseMinMax((Vector2)min2 * 0.1f, (Vector2)max2 * 0.1f);

        var newMin = height.min + subHeight.min * 0.05f;
        var newMax = height.max + subHeight.max * 0.05f;

        newMin += baseHeight.min * 2;
        newMax += baseHeight.max * 2;

        newMin *= (baseHeight.min + 1);
        newMax *= (baseHeight.max + 1);

        _cacheValue = (ModifyHeight(newMin), ModifyHeight(newMax));
        return _cacheValue;
    }

    private float ModifyHeight(float value)
    {
        const int baseHeight = 0;
        const int amplitude  = 25;
        return baseHeight + value * amplitude;
    }
}