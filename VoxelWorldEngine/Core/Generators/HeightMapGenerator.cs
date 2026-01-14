using DotnetNoise;
using VoxelWorldEngine.Core.Generators.Interfaces;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core.Generators;

// TODO: write doc

public class HeightMapGenerator : IScalarFieldGenerator<Vector2Int, float>
{
    
    private readonly FastNoise _noise;
    private readonly float _multiplier;
    
    public HeightMapGenerator(FastNoise noise)
    {
        _noise = noise;
        _noise.UsedNoiseType = FastNoise.NoiseType.Simplex;
        _noise.FractalTypeMethod = FastNoise.FractalType.Fbm;
        _noise.Octaves = 1;
        _noise.Frequency = 0.01f;
        _noise.Gain = 0.5f;
        _noise.Lacunarity = 2f;
        
        _multiplier = 25f;
    }

    public float GetValue(Vector2Int position)
    {
        var height = _noise.GetNoise(position.X, position.Y);

        return height * _multiplier;
    }

    public (float min, float max) GetMinMax(Vector2Int a, Vector2Int b)
    {
        var height = _noise.GetNoiseMinMax(a, b);
        return (height.min * _multiplier, height.max * _multiplier);
    }

    public bool IsHereAnySurface(Vector2Int a, Vector2Int b)
    {
        return true;
    }
}

