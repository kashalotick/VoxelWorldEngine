using System.Numerics;
using DotnetNoise;
using Vector2Int = VoxelModule.Core.Vectors.Vector2Int;
using Vectors_Vector2Int = VoxelModule.Core.Vectors.Vector2Int;

namespace VoxelModule.Utils;

public static class FastNoiseExtensions
{
    public static (float min, float max) GetNoiseMinMax(this FastNoise noise, Vectors_Vector2Int aInt, Vectors_Vector2Int bInt)
    {
        var a = new Vector2(aInt.X, aInt.Y);
        var b = new Vector2(bInt.X, bInt.Y);
        return GetNoiseMinMax(noise, a, b);
    }

    public static (float min, float max) GetNoiseMinMax(this FastNoise noise, Vector2 a, Vector2 b)
    {
        return GetApproximateRange(noise, a, b);
    }

    public static (float min, float max) GetApproximateRange(
        this FastNoise noise,
        Vectors_Vector2Int minInt,
        Vectors_Vector2Int maxInt,
        int initialStep = 16,
        int refinementSteps = 3,
        float finalDensityFactor = 0.004f
    )
    {
        var min = new Vector2(minInt.X, minInt.Y);
        var max = new Vector2(maxInt.X, maxInt.Y);
        return GetApproximateRange(noise, min, max, initialStep, refinementSteps, finalDensityFactor);
    }

    public static (float min, float max) GetApproximateRange(
        this FastNoise noise,
        Vector2 min,
        Vector2 max,
        int initialStep = 16,
        int refinementSteps = 3,
        float finalDensityFactor = 0.004f)
    {
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        int step = initialStep;
        SampleRegion(noise, min, max, step, ref minValue, ref maxValue);

        for (int i = 0; i < refinementSteps; i++)
        {
            step = Math.Max(1, step / 3);
            SampleRegion(noise, min, max, step, ref minValue, ref maxValue);
        }

        if (finalDensityFactor > 0)
        {
            float width = max.X - min.X;
            float height = max.Y - min.Y;
            int finalStep = Math.Max(1, (int)Math.Round(
                Math.Sqrt(width * height) * finalDensityFactor));
            
            SampleRegion(noise, min, max, finalStep, ref minValue, ref maxValue);
        }

        return (minValue, maxValue);
    }

    private static void SampleRegion(
        FastNoise noise,
        Vector2 min,
        Vector2 max,
        int step,
        ref float minValue,
        ref float maxValue)
    {
        for (float x = min.X; x <= max.X; x += step)
        {
            for (float y = min.Y; y <= max.Y; y += step)
            {
                float value = noise.GetNoise(x, y);
                
                if (value < minValue)
                    minValue = value;
                if (value > maxValue)
                    maxValue = value;
            }
        }
    }

    


}