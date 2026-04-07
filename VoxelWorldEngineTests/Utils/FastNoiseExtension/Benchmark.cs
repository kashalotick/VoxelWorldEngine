using System.Diagnostics;
using DotnetNoise;
using VoxelWorldEngine.Utils;
using Vector2Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector2Int;

namespace VoxelWorldEngineTests.Utils.FastNoiseExtension;

public class Benchmark
{
    [Test]
    public void FastNoise_MinMax_Benchmark()
    {
        var noise = new FastNoise(123)
        {
            UsedNoiseType = FastNoise.NoiseType.Simplex,
            FractalTypeMethod = FastNoise.FractalType.Fbm,
            Octaves = 1,
            Frequency = 0.01f,
            Gain = 0.5f,
            Lacunarity = 2f
        };

        var initialStep = 16;
        var refinementSteps = 1;
        var finalDensityFactor = 0.002f;
        var iterations = 256 * 4;
        var min = 32;
        var max = 256;
        RunBenchmark(noise, iterations, min, max, initialStep, refinementSteps, finalDensityFactor);
    }

    public static void RunBenchmark(
        FastNoise noise,
        int iterations = 100,
        int minAreaSize = 10,
        int maxAreaSize = 100,
        int initialStep = 16,
        int refinementSteps = 3,
        float finalDensityFactor = 0.004f
    )
    {
        double totalTimeMs = 0;
        float totalErrorMin = 0;
        float totalErrorMax = 0;
        float maxSingleError = 0;

        Console.WriteLine($"--- Starting Benchmark: {iterations} iterations ---");

        var random = new Random();
        for (var i = 0; i < iterations; i++)
        {
            // 1. Генеруємо рандомну область
            var start = new Vector2Int(random.Next(-10000, 10000),
                random.Next(-10000, 10000));
            var size = new Vector2Int(random.Next(minAreaSize, maxAreaSize),
                random.Next(minAreaSize, maxAreaSize));
            var end = start + size;

            // 2. Вимірюємо час виконання твого методу
            var sw = Stopwatch.StartNew();
            var result = noise.GetApproximateRange(start, end, initialStep, refinementSteps, finalDensityFactor);
            sw.Stop();
            totalTimeMs += sw.Elapsed.TotalMilliseconds;

            // 3. Рахуємо реальні значення (Brute Force) для перевірки похибки
            var realValues = GetNoiseMinMaxBruteForce(noise, start, end);

            // 4. Рахуємо похибку
            var errorMin = Math.Abs(result.min - realValues.min);
            var errorMax = Math.Abs(result.max - realValues.max);

            totalErrorMin += errorMin;
            totalErrorMax += errorMax;
            maxSingleError = Math.Max(maxSingleError, Math.Max(errorMin, errorMax));
        }

        // Підбиваємо підсумки
        var avgTime = totalTimeMs / iterations;
        var avgErrorMin = totalErrorMin / iterations;
        var avgErrorMax = totalErrorMax / iterations;

        Console.WriteLine("\n--- RESULTS ---");
        Console.WriteLine($"Total Time: {totalTimeMs:F4} ms");
        Console.WriteLine($"Average Time per call: {avgTime:F6} ms");
        Console.WriteLine($"Average Error (MinIndex): {avgErrorMin:F6}");
        Console.WriteLine($"Average Error (MaxIndex): {avgErrorMax:F6}");
        Console.WriteLine($"MaxIndex Single Error found: {maxSingleError:F6}");

        if (maxSingleError < 0.00001f)
            Console.WriteLine("Accuracy: Perfect (or very high)");
        else
            Console.WriteLine("Accuracy: Contains approximations");
        Console.WriteLine("----------------\n");
    }

    // Еталонний метод (перебір усіх точок)
    private static (float min, float max) GetNoiseMinMaxBruteForce(FastNoise noise, Vector2Int a, Vector2Int b)
    {
        var min = float.MaxValue;
        var max = float.MinValue;

        var startX = Math.Min(a.X, b.X);
        var endX = Math.Max(a.X, b.X);
        var startY = Math.Min(a.Y, b.Y);
        var endY = Math.Max(a.Y, b.Y);

        for (var x = startX; x <= endX; x++)
        {
            for (var y = startY; y <= endY; y++)
            {
                var val = noise.GetNoise(x, y);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        return (min, max);
    }
}