using System;
using System.Collections.Generic;
using DotnetNoise;
using VoxelModule.Core;
using VoxelModule.Engine;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Content.Generators;

public class VegetationGenerator
{
    private readonly SurfaceGenerator _generator;
    private readonly int _chunkSize; 
    private readonly int _borderRadius;
    private readonly int _seed;
    private FastNoise _forestNoise;

    
    /// <summary>
    /// Масштаб лісових масивів та полів. 
    /// Зменшуй для величезних лісів (0.005f - 0.001f).
    /// Збільшуй для дрібних "острівців" (0.03f+).
    /// </summary>
    public float ForestNoiseFrequency { get; set; } = 0.005f;

    /// <summary>
    /// Поріг появи лісу (від 0.0 до 1.0).
    /// Чим вище значення, тим більше буде відкритих полів і рідше траплятимуться ліси.
    /// </summary>
    public float ForestThreshold { get; set; } = 0.55f;

    /// <summary>
    /// Максимальна густота лісу (шанс появи дерева на 1 блок) у найгустіших хащах.
    /// 0.05f = 5% блоків будуть з деревами.
    /// </summary>
    public float MaxTreeDensity { get; set; } = 0.03f;

    /// <summary>
    /// Плавність переходу від краю лісу до його центру. 
    /// Визначає, наскільки швидко густота дерев досягає MaxTreeDensity після перетину ForestThreshold.
    /// </summary>
    public float EdgeFadeDistance { get; set; } = 0.3f;

    /// <summary>
    /// Максимальна висота (Y), де ще можуть рости дерева (щоб на вершинах гір було пусто).
    /// </summary>
    public int MaxTreeElevation { get; set; } = 35;


    public VegetationGenerator(SurfaceGenerator generator, int chunkSize, int borderRadius, int seed)
    {
        _generator = generator;
        _chunkSize = chunkSize;
        _borderRadius = borderRadius;
        _seed = seed;
        
        InitNoise();
    }
    
    public void InitNoise()
    {
        _forestNoise = new FastNoise(_seed + 99)
        {
            Frequency = ForestNoiseFrequency,
        };
    }

    public List<Vector3Int> GetTreeSpawnPoints(Vector3Int chunkPos)
    {
        var result = new List<Vector3Int>();

        int worldOriginX = chunkPos.X * _chunkSize;
        int worldOriginZ = chunkPos.Z * _chunkSize;

        int from = -_borderRadius;
        int to   =  _chunkSize + _borderRadius;

        for (int lx = from; lx < to; lx++)
        {
            for (int lz = from; lz < to; lz++)
            {
                int worldX = worldOriginX + lx;
                int worldZ = worldOriginZ + lz;

                if (!ShouldSpawnTree(worldX, worldZ))
                    continue;

                _generator.Offset = Vector3Int.Zero; 
                int surfaceY = _generator.GetSurfaceY(worldX, worldZ);
                
                if (surfaceY > MaxTreeElevation) 
                    continue;

                var ground = _generator.Approximate(new Vector3Int(worldX, surfaceY, worldZ), new Vector3Int(worldX, surfaceY, worldZ));
                if (ground.BlockId != BlockId.Grass)
                    continue;

                result.Add(new Vector3Int(worldX, surfaceY + 1, worldZ));
            }
        }

        return result;
    }

    private bool ShouldSpawnTree(int worldX, int worldZ)
    {
        float noiseVal = _forestNoise.GetNoise(worldX, worldZ); 
        float forestMask = (noiseVal + 1f) * 0.5f; 
    
        if (forestMask < ForestThreshold)
            return false;

        float depthInForest = forestMask - ForestThreshold;
        
        float densityFactor = Math.Min(depthInForest / EdgeFadeDistance, 1.0f); 
        float localDensity = MaxTreeDensity * densityFactor;

        uint hash = (uint)(_seed ^ (worldX * 374761393) ^ (worldZ * 1000003));
        hash ^= hash >> 13;
        hash *= 1274126177;
        hash ^= hash >> 16;

        uint threshold = (uint)(localDensity * 1000f); 

        return (hash % 1000) < threshold;
    }
}