using DotnetNoise;
using VoxelModule.Engine.Octree;
using VoxelModule.Utils;
using Vector2Int = VoxelModule.Core.Vectors.Vector2Int;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine;

public class SurfaceGenerator : IGenerator
{
    public Vector3Int Offset;
    private FastNoise _noise;
    private FastNoise _noise2;
    private FastNoise _noiseBase;
    private FastNoise _caveNoise;

    private const int MaxDirtDepth = 4;
    private const int MaxGrassDepth = 1;
    private const int MaxCaveDepth = -64;
    private const float CaveThreshold = 0.07f;
    
    private (Vector2Int min, Vector2Int max) _cacheVector;
    private (float min, float max) _cacheValue;

    public SurfaceGenerator(int seed, Vector3Int offset)
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

        if (max.Y <= height.min - GetDirtDepth(height.min) + MaxCaveDepth)
            return new Voxel(BlockId.Stone);

        if (min.Y > height.max)
            return new Voxel(BlockId.Air);

        var centerY = (min.Y + max.Y) / 2f;
        var centerHeight = (height.min + height.max) / 2f;

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

        if (dirtDepth == 0)
            return new Voxel(BlockId.Stone);

        if (centerY > surfaceHeight - MaxGrassDepth)
            return new Voxel(BlockId.Grass);
        
        if (centerY > surfaceHeight - dirtDepth)
            return new Voxel(BlockId.Dirt);

        return new Voxel(BlockId.Stone);
    }
    public bool IsUniform(Vector3Int min, Vector3Int max)
    {
        min += Offset;
        max += Offset;

        var height = Evaluate(min, max);

        if (min.Y > height.max)
            return true;

        if (max.Y <= height.min - MaxDirtDepth)
        {
            if (max.Y <= height.min - MaxDirtDepth + MaxCaveDepth)
                return true;

            if (!MightContainCave(min, max))
                return true;
        }

        return false;
    }
    private bool MightContainCave(Vector3Int worldMin, Vector3Int worldMax)
    {
        float cx = (worldMin.X + worldMax.X) * 0.5f;
        float cy = (worldMin.Y + worldMax.Y) * 0.5f;
        float cz = (worldMin.Z + worldMax.Z) * 0.5f;

        float dx = worldMax.X - cx;
        float dy = worldMax.Y - cy;
        float dz = worldMax.Z - cz;
        float radius = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

        float maxVariation = radius * 0.05f;

        float n1 = _caveNoise.GetNoise(cx, cy, cz);
        if (MathF.Abs(n1) - maxVariation > CaveThreshold) 
            return false;

        float n2 = _caveNoise.GetNoise(cx + 1000, cy + 500, cz + 1000);
        if (MathF.Abs(n2) - maxVariation > CaveThreshold) 
            return false;
        
        return true; 
    }
    private bool IsCave(Vector3Int worldPos)
    {
        float n1 = _caveNoise.GetNoise(worldPos.X, worldPos.Y, worldPos.Z);
        float n2 = _caveNoise.GetNoise(worldPos.X + 1000, worldPos.Y + 500, worldPos.Z + 1000);

        return MathF.Abs(n1) < CaveThreshold && MathF.Abs(n2) < CaveThreshold;
    }
    private int GetDirtDepth(float surfaceHeight)
    {
        const float minHeightForThinDirt = 20f;
        const float maxHeightForStone = 45f;

        if (surfaceHeight <= minHeightForThinDirt)
            return MaxDirtDepth;

        if (surfaceHeight >= maxHeightForStone)
            return 0;

        float t = (surfaceHeight - minHeightForThinDirt) / 
                  (maxHeightForStone - minHeightForThinDirt);
    
        float depthRaw = MaxDirtDepth * (1f - t * 1.1f);
        return (int)MathF.Max(0, MathF.Round(depthRaw)); 
    }
    private (float min, float max) Evaluate(Vector3Int min, Vector3Int max)
    {
        var min2 = new Vector2Int(min.X, min.Z);
        var max2 = new Vector2Int(max.X, max.Z);

        const int absoluteMax = 156;

        if (min.Y >= absoluteMax) return (absoluteMax, absoluteMax);

        if (_cacheVector == (min2, max2))
            return _cacheValue;

        _cacheVector = (min2, max2);

        var height     = _noise.GetNoiseMinMax(min2, max2);
        var subHeight  = _noise2.GetNoiseMinMax(min2 * 3, max2 * 3);
        var baseHeight = _noiseBase.GetNoiseMinMax(min2, max2);

        var newMin = height.min + subHeight.min * 0.05f;
        var newMax = height.max + subHeight.max * 0.05f;

        newMin += baseHeight.min * 2;
        newMax += baseHeight.max * 2;

        newMin *= (baseHeight.min * 0.5f + 1f);
        newMax *= (baseHeight.max * 0.5f + 1f);


        _cacheValue = (ModifyHeight(newMin), ModifyHeight(newMax));
        return _cacheValue;
    }

    public int GetSurfaceY(int worldX, int worldZ)
    {
        var point = new Vector2Int(worldX, worldZ);
        var height = _noise.GetNoiseMinMax(point, point);
        var subHeight = _noise2.GetNoiseMinMax(point * 3, point * 3);
        var baseHeight = _noiseBase.GetNoiseMinMax(point, point);

        float h = height.min + subHeight.min * 0.05f;
        h += baseHeight.min * 2;
        h *= (baseHeight.min * 0.5f + 1f);

        return (int)MathF.Floor(ModifyHeight(h));
    }
    
    internal float ModifyHeight(float value)
    {
        const int baseHeight = 0;
        const int amplitude  = 25;
        return baseHeight + value * amplitude;
    }
}