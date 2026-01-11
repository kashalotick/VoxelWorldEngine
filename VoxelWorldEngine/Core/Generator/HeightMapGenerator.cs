using DotnetNoise;
using VoxelWorldEngine.Core.Generator.Interfaces;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core.Generator;

public class HeightMapGenerator : IScalarFieldGenerator<Vector2Int, float>
{
    
    private readonly FastNoise _noise;
    
    public HeightMapGenerator()
    {
        _noise = new FastNoise(123);
        _noise.UsedNoiseType = FastNoise.NoiseType.Simplex;
        _noise.FractalTypeMethod = FastNoise.FractalType.Fbm;
        _noise.Octaves = 1;
        _noise.Frequency = 0.01f;
        _noise.Gain = 0.5f;
        _noise.Lacunarity = 2f;

    }

    public float GetValue(Vector2Int position)
    {
        var height = _noise.GetNoise(position.X, position.Y);

        return height;
    }

    public (float min, float max) GetMinMax(Vector2Int a, Vector2Int b)
    {
        return _noise.GetNoiseMinMax(a, b);
    }


}

