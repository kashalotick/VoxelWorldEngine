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
    private FastNoise _noiseBase;
    private FastNoise _caveNoise;

    private const int MaxDirtDepth = 4;     // максимальна глибина землі
    private const int MaxGrassDepth = 1;
    private const int MaxCaveDepth = -64;
    private const float CaveThreshold = 0.07f;
    
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
        _caveNoise = new FastNoise(seed + 42)
        {
            Frequency = 0.009f,
            Lacunarity = 2f,
            Gain = 0.5f,
        };
        _noiseBase = new FastNoise(seed + 1) { Frequency = 0.002f };
    }

    public Voxel Approximate(Vector3Int min, Vector3Int max)
    {
        min += Offset;
        max += Offset;

        var height = Evaluate(min, max);

        // 1. Повністю під землею — камінь
        if (max.Y <= height.min - GetDirtDepth(height.min) + MaxCaveDepth)
            return new Voxel(BlockId.Stone);

        // 2. Повністю над поверхнею — повітря
        if (min.Y > height.max)
            return new Voxel(BlockId.Air);

        var centerY = (min.Y + max.Y) / 2f;
        var centerHeight = (height.min + height.max) / 2f;

        // Повітря
        if (centerY > centerHeight)
            return new Voxel(BlockId.Air);

        var centerPos = new Vector3Int((int)((min.X + max.X) / 2f),
            (int)centerY,
            (int)((min.Z + max.Z) / 2f));

        
        float depthBelowSurface = centerHeight - centerY;
        if (depthBelowSurface >= 0 && IsCave(centerPos))
            return new Voxel(BlockId.Air);
        
        float surfaceHeight = centerHeight;
        int dirtDepth = GetDirtDepth(surfaceHeight);

        // ВИПРАВЛЕННЯ: Якщо шар ґрунту = 0 (на вершинах гір), має бути ТІЛЬКИ камінь, без трави
        if (dirtDepth == 0)
            return new Voxel(BlockId.Stone);

        // Трава — самий верх (товщиною 1)
        if (centerY > surfaceHeight - MaxGrassDepth)
            return new Voxel(BlockId.Grass);
        
        // Земля
        if (centerY > surfaceHeight - dirtDepth)
            return new Voxel(BlockId.Dirt);

        // Все нижче — камінь
        return new Voxel(BlockId.Stone);
    }
    public bool IsUniform(Vector3Int min, Vector3Int max)
    {
        min += Offset;
        max += Offset;

        var height = Evaluate(min, max);

        int dirtDepth = GetDirtDepth((height.min + height.max) / 2f);
        // int dirtDepth = MaxDirtDepth; // worst case — не скорочуй тут

        if (max.Y <= height.min - dirtDepth + MaxCaveDepth)
            return true;

        if (min.Y > height.max)
            return true;

        return false;
    }
    private bool IsCave(Vector3Int worldPos)
    {
        // Спагетті печери — два незалежних noise з різними offset'ами
        // Печера там, де ОБИДВА близькі до 0 (трубка = перетин двох "сфер")
        float n1 = _caveNoise.GetNoise(worldPos.X, worldPos.Y, worldPos.Z);
        float n2 = _caveNoise.GetNoise(worldPos.X + 1000, worldPos.Y + 500, worldPos.Z + 1000);

        return MathF.Abs(n1) < CaveThreshold && MathF.Abs(n2) < CaveThreshold;
    }
    // НОВА ФУНКЦІЯ — глибина землі залежить від висоти поверхні
    private int GetDirtDepth(float surfaceHeight)
    {
        const float minHeightForThinDirt = 20f;
        const float maxHeightForStone = 45f;

        if (surfaceHeight <= minHeightForThinDirt)
            return MaxDirtDepth;                    // рівнина — нормальна земля 4 блоки

        if (surfaceHeight >= maxHeightForStone)
            return 0;                               // дуже високі гори — чисто камінь зверху

        float t = (surfaceHeight - minHeightForThinDirt) / 
                  (maxHeightForStone - minHeightForThinDirt);
    
        // ВИПРАВЛЕННЯ: MathF.Round зберігає товщину шару більш природно. 
        // Значення 1.5 - 1.8 на висоті 32-34 округлюватимуться до 2 (1 трава + 1 земля).
        float depthRaw = MaxDirtDepth * (1f - t * 1.1f);
        return (int)MathF.Max(0, MathF.Round(depthRaw)); 
    }
    private (float min, float max) Evaluate(Vector3Int min, Vector3Int max)
    {
        var min2 = new Vector2Int(min.X, min.Z);
        var max2 = new Vector2Int(max.X, max.Z);

        // const int absoluteMin = -32;
        const int absoluteMax = 156;

        // if (max.Y <= absoluteMin) return (absoluteMin, absoluteMin);
        if (min.Y >= absoluteMax) return (absoluteMax, absoluteMax);

        if (_cacheVector == (min2, max2))
            return _cacheValue;

        _cacheVector = (min2, max2);

        // var warpX = _noiseWarp.GetNoise(min.X, min.Z) * 30f;
        // var warpZ = _noiseWarp.GetNoise(min.X + 100, min.Z + 100) * 30f;
        //
        // var height = _noise.GetNoiseMinMax(
        //     new Vector2Int(min.X + (int)warpX, min.Z + (int)warpZ),
        //     new Vector2Int(max.X + (int)warpX, max.Z + (int)warpZ)
        // );
        var height     = _noise.GetNoiseMinMax(min2, max2);
        var subHeight  = _noise2.GetNoiseMinMax(min2 * 3, max2 * 3);
        var baseHeight = _noiseBase.GetNoiseMinMax(min2, max2);

        var newMin = height.min + subHeight.min * 0.05f;
        var newMax = height.max + subHeight.max * 0.05f;

        newMin += baseHeight.min * 2;
        newMax += baseHeight.max * 2;

        newMin *= (baseHeight.min * 0.5f + 1f);
        newMax *= (baseHeight.max * 0.5f + 1f);

        // newMin *= (baseHeight.min + 1);
        // newMax *= (baseHeight.max + 1);

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