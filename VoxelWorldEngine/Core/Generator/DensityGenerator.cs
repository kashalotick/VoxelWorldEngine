using VoxelWorldEngine.Core.Generator.Interfaces;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Core.Generator;

public class DensityGenerator : IScalarFieldGenerator<Vector3Int, int>
{
    private HeightMapGenerator _heightMap;
    private (Vector2Int Position, float Value) _previousHeight;
    private (Vector2Int a, Vector2Int b, (float min, float max) Value) _previousHeightMinMax;


    // temp constants
    private const int Solid = 1;
    private const int Air = 0;


    public DensityGenerator()
    {
        _heightMap = new HeightMapGenerator();
    }

    public int GetValue(Vector3Int position)
    {
        var vec2 = new Vector2Int(position.X, position.Y);
        var height = GetHeightValue(vec2);

        var density = position.Z <= height ? Solid : Air;
        return density;
    }


    public (int min, int max) GetMinMax(Vector3Int a, Vector3Int b)
    {
        var vecA2 = new Vector2Int(a.X, a.Y);
        var vecB2 = new Vector2Int(b.X, b.Y);

        var zMax = a.Z > b.Z ? a.Z : b.Z;
        var zMin = a.Z < b.Z ? a.Z : b.Z;
        (int min, int max) z = (zMin, zMax);

        var height = GetHeightMinMax(vecA2, vecB2);

        if (z.max < height.min) return (Solid, Solid);
        if (z.min > height.max) return (Air, Air);
        return (Solid, Air);
    }

    public bool IsHereAnySurface(Vector3Int a, Vector3Int b)
    {
        var density = GetMinMax(a, b);
        
        if (density.min >= 1) return false;
        if (density.max <= 0) return false;

        return true;
    }

    private float GetHeightValue(Vector2Int vec2)
    {
        float height;
        if (vec2 == _previousHeight.Position)
        {
            height = _previousHeight.Value;
        }
        else
        {
            height = _heightMap.GetValue(vec2);
            _previousHeight = (vec2, height);
        }

        return height;
    }


    private (float min, float max) GetHeightMinMax(Vector2Int vecA2, Vector2Int vecB2)
    {
        (float min, float max) heightMinMax;
        if (vecA2 == _previousHeightMinMax.a && vecB2 == _previousHeightMinMax.b)
        {
            heightMinMax = _previousHeightMinMax.Value;
        }
        else
        {
            heightMinMax = _heightMap.GetMinMax(vecA2, vecB2);
            _previousHeightMinMax = (vecA2, vecB2, heightMinMax);
        }

        return heightMinMax;
    }
}