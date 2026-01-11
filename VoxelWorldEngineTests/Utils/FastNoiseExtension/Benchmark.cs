using System.Diagnostics;
using DotnetNoise;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.Utils;

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
        for (int i = 0; i < iterations; i++)
        {
            // 1. Генеруємо рандомну область
            Vector2Int start = new Vector2Int(random.Next(-10000, 10000),
                random.Next(-10000, 10000));
            Vector2Int size = new Vector2Int(random.Next(minAreaSize, maxAreaSize),
                random.Next(minAreaSize, maxAreaSize));
            Vector2Int end = start + size;

            // 2. Вимірюємо час виконання твого методу
            Stopwatch sw = Stopwatch.StartNew();
            var result = noise.GetApproximateRange(start, end, initialStep, refinementSteps, finalDensityFactor);
            sw.Stop();
            totalTimeMs += sw.Elapsed.TotalMilliseconds;

            // 3. Рахуємо реальні значення (Brute Force) для перевірки похибки
            var realValues = GetNoiseMinMaxBruteForce(noise, start, end);

            // 4. Рахуємо похибку
            float errorMin = Math.Abs(result.min - realValues.min);
            float errorMax = Math.Abs(result.max - realValues.max);

            totalErrorMin += errorMin;
            totalErrorMax += errorMax;
            maxSingleError = Math.Max(maxSingleError, Math.Max(errorMin, errorMax));
        }

        // Підбиваємо підсумки
        double avgTime = totalTimeMs / iterations;
        float avgErrorMin = totalErrorMin / iterations;
        float avgErrorMax = totalErrorMax / iterations;

        Console.WriteLine("\n--- RESULTS ---");
        Console.WriteLine($"Total Time: {totalTimeMs:F4} ms");
        Console.WriteLine($"Average Time per call: {avgTime:F6} ms");
        Console.WriteLine($"Average Error (Min): {avgErrorMin:F6}");
        Console.WriteLine($"Average Error (Max): {avgErrorMax:F6}");
        Console.WriteLine($"Max Single Error found: {maxSingleError:F6}");

        if (maxSingleError < 0.00001f)
            Console.WriteLine("Accuracy: Perfect (or very high)");
        else
            Console.WriteLine("Accuracy: Contains approximations");
        Console.WriteLine("----------------\n");
    }

    // Еталонний метод (перебір усіх точок)
    private static (float min, float max) GetNoiseMinMaxBruteForce(FastNoise noise, Vector2Int a, Vector2Int b)
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        int startX = Math.Min(a.X, b.X);
        int endX = Math.Max(a.X, b.X);
        int startY = Math.Min(a.Y, b.Y);
        int endY = Math.Max(a.Y, b.Y);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                float val = noise.GetNoise(x, y);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        return (min, max);
    }
}