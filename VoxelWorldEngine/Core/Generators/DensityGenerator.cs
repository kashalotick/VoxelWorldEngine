using VoxelWorldEngine.Core.Generators.Interfaces;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Core.Generators;


// TODO: write doc
public class DensityGenerator : IScalarFieldGenerator<Vector3Int, sbyte>
{
    private readonly IScalarFieldGenerator<Vector2Int, float> _heightMap;
    
    // TODO: remake with chunk (256x256) memoization; bring it in HeightMapGenerator
    // private (Vector2Int Position, float Value) _previousHeight;
    // private (Vector2Int a, Vector2Int b, (float min, float max) Value) _previousHeightMinMax;


    // temp constants
    private const sbyte Solid = sbyte.MaxValue;
    private const sbyte Air = sbyte.MinValue;


    public DensityGenerator(IScalarFieldGenerator<Vector2Int, float> heightMapGenerator)
    {
        _heightMap = heightMapGenerator;
    }

    public sbyte GetValue(Vector3Int position)
    {
        var vec2 = new Vector2Int(position.X, position.Y);
        var height = _heightMap.GetValue(vec2);
        
        var density = position.Z <= height ? Solid : Air;
        return density;
    }
    
    public (sbyte min, sbyte max) GetMinMax(Vector3Int a, Vector3Int b)
    {
        var vecA2 = new Vector2Int(a.X, a.Y);
        var vecB2 = new Vector2Int(b.X, b.Y);
        // TODO: working but still disappearing faces
        // if (Vector3Int.Abs(a - b) == Vector3Int.One)
        // {
        //     var density = GetValue(a);
        //     return (density, density);
        // }
      

        var zMax = Math.Max(a.Z, b.Z);
        var zMin = Math.Min(a.Z, b.Z);

        var height = _heightMap.GetMinMax(vecA2, vecB2);

        if (zMax <= height.min) return (Solid, Solid);
        if (zMin > height.max) return (Air, Air);
        return (Air, Solid);
    }

    public bool IsHereAnySurface(Vector3Int a, Vector3Int b)
    {
        var density = GetMinMax(a, b);
        
        if (density.min >= 0) return false;
        if (density.max < 0) return false;

        return true;
    }

    // private float GetHeightValue(Vector2Int vec2)
    // {
    //     float height;
    //     if (vec2 == _previousHeight.Position)
    //     {
    //         height = _previousHeight.Value;
    //     }
    //     else
    //     {
    //         height = _heightMap.GetValue(vec2);
    //         _previousHeight = (vec2, height);
    //     }
    //
    //     return height;
    // }
    //
    //
    // private (float min, float max) GetHeightMinMax(Vector2Int vecA2, Vector2Int vecB2)
    // {
    //     (float min, float max) heightMinMax;
    //     if (vecA2 == _previousHeightMinMax.a && vecB2 == _previousHeightMinMax.b)
    //     {
    //         heightMinMax = _previousHeightMinMax.Value;
    //     }
    //     else
    //     {
    //         heightMinMax = _heightMap.GetMinMax(vecA2, vecB2);
    //         _previousHeightMinMax = (vecA2, vecB2, heightMinMax);
    //     }
    //
    //     return heightMinMax;
    // }
}