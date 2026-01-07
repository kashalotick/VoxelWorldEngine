using DotnetNoise;
using VoxelWorldEngine.Core.Generator.Interfaces;
using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Generator;

public class SimplexSurfaceGenerator : IScalarFieldGenerator<int>
{
    private const int Multiplier = 32;
    private readonly FastNoise _noise;

    private (int x, int y) _previousPosition;
    private float _cachedHeight;

    
    public SimplexSurfaceGenerator()
    {
        _noise = new FastNoise(1234);
        _noise.Frequency = 0.1f;
    }
    
    public int GetValue(Vector3Int position)
    {
        var height = GetSurfaceHeight(position.X, position.Y);

        if (position.Z < height)
        {
            return 1;
        }

        return 0;
    }

    public int GetMinimum(Vector3Int a, Vector3Int b)
    {
        var min = -Multiplier;
        var maxZ = Math.Max(a.Z, b.Z);

        return maxZ < min ? 0 : -1;
    }

    public int GetMaximum(Vector3Int a, Vector3Int b)
    {
        var max = Multiplier;
        var minZ = Math.Min(a.Z, b.Z);

        return minZ > max ? 0 : 1;
    }
    
    public float GetSurfaceHeight(int x, int y)
    {
        if ((x, y) == _previousPosition)
        {
            return _cachedHeight;
        }

        var height = _noise.GetSimplex(x, y) * Multiplier;
        _previousPosition = (x, y);
        _cachedHeight = height;
        return height;
    }
}