using DotnetNoise;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core.Generatros;

public class NoiseGenerator(FastNoise noise) : IGenerator<float, Vector2Int>
{
    private readonly FastNoise _noise = noise;

    public float Approximate(Vector2Int min, Vector2Int max)
    {
        throw new NotImplementedException();
    }

    public bool IsUniform(Vector2Int min, Vector2Int max)
    {
        throw new NotImplementedException();
    }

    public float Maximum(Vector2Int min, Vector2Int max)
    {
        throw new NotImplementedException();
    }

    public float Minimum(Vector2Int min, Vector2Int max)
    {
        throw new NotImplementedException();
    }
    
    private void Evaluate(Vector2Int min, Vector2Int max)
    {
        
    }
}