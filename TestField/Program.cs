using System.Diagnostics;
using VoxelWorldEngine;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace TestField;

class Program
{
    static void Main(string[] args)
    {
        var root = "F:/Univer/Rider/LearningOpenTK/LearningOpenTK";

        var dir = new DirectoryInfo(root);
        Console.WriteLine(dir.FullName);
        var files = dir.EnumerateFiles("*.cs", SearchOption.AllDirectories);
        Console.WriteLine(string.Join('\n', files.Select(f => f.Name)));
        return;
        var seed = 151234;
        var iterations = 100;
        // Console.WriteLine("\n-- Top surface chunk");
        // ProfileBuildMethod(seed, new Vector3Int(0, 0, 0), iterations);
        // Console.WriteLine("\n-- Down surface chunk");
        // ProfileBuildMethod(seed, new Vector3Int(0, -1, 0), iterations);
        
        ProfileBuildAreaMethod(seed, new Vector3Int(-5, 0, -5), new Vector3Int(5, 1, 5));
        Counter.Display();
    }
    

    private static void ProfileBuildMethod(int seed, Vector3Int position, int iterations = 100)
    {
        // Накопичувачі часу для кожного етапу (в мілісекундах)
        double totalGenTime = 0;
        double totalOctreeTime = 0;
        double totalMeshTime = 0;
        double totalChunkTime = 0;

        Stopwatch sw = new Stopwatch();

        Console.WriteLine($"Початок профілювання: {iterations} ітерацій...");

        for (int i = 0; i < iterations; i++)
        {
            Counter.Increment(CounterType.VoxelOctreeBuild);

            // 1. Етап Генератора
            sw.Restart();
            IGenerator generator = new ProceduralGenerator(seed, Chunk.ChunkToGlobal(position));
            sw.Stop();
            totalGenTime += sw.Elapsed.TotalMilliseconds;

            // 2. Етап Побудови Octree
            sw.Restart();
            var octree = new VoxelOctree();
            octree.Build(generator);
            sw.Stop();
            totalOctreeTime += sw.Elapsed.TotalMilliseconds;

            // 3. Етап Генерації Mesh
            sw.Restart();
            var meshBuilder = new MeshBuilder();
            var mesh = meshBuilder.Build(octree);
            sw.Stop();
            totalMeshTime += sw.Elapsed.TotalMilliseconds;

            // 4. Етап Створення об'єкта Chunk
            sw.Restart();
            var chunk = new Chunk(position);
            chunk.Octree = octree;
            chunk.Mesh = mesh;
            sw.Stop();
            totalChunkTime += sw.Elapsed.TotalMilliseconds;
        }

        // Вивід результатів
        Console.WriteLine("--- Результати (Середній час на етап) ---");
        PrintResult("Generator Setup", totalGenTime, iterations);
        PrintResult("Octree Build   ", totalOctreeTime, iterations);
        PrintResult("Mesh Building  ", totalMeshTime, iterations);
        PrintResult("Chunk Instance ", totalChunkTime, iterations);

        double grandTotal = totalGenTime + totalOctreeTime + totalMeshTime + totalChunkTime;
        double grandAverage = grandTotal / iterations;
        Console.WriteLine($"Загальний час: {grandTotal:N2} ms");
        Console.WriteLine($"Загальний середній час: {grandAverage:F4} ms");
    }

    private static void ProfileBuildAreaMethod(int seed, Vector3Int min, Vector3Int max)
    {
        // Накопичувачі часу для кожного етапу (в мілісекундах)
        double totalGenTime = 0;
        double totalOctreeTime = 0;
        double totalMeshTime = 0;
        double totalChunkTime = 0;

        Stopwatch sw = new Stopwatch();

        var iterations = (max.X - min.X) * (max.Y - min.Y) * (max.Z - min.Z);
        Console.WriteLine($"Початок профілювання: AABB min={min} max={max} ({iterations} чанків)...");

        for (int x = 0; x < max.X - min.X; x++)
        for (int y = 0; y < max.Y - min.Y; y++)
        for (int z = 0; z < max.Z - min.Z; z++)

        {
            var position = new Vector3Int(min.X + x, min.Y + y, min.Z + z);
            Counter.Increment(CounterType.VoxelOctreeBuild);

            // 1. Етап Генератора
            sw.Restart();
            IGenerator generator = new ProceduralGenerator(seed, Chunk.ChunkToGlobal(position));
            sw.Stop();
            totalGenTime += sw.Elapsed.TotalMilliseconds;

            // 2. Етап Побудови Octree
            sw.Restart();
            var octree = new VoxelOctree();
            octree.Build(generator);
            sw.Stop();
            totalOctreeTime += sw.Elapsed.TotalMilliseconds;

            // 3. Етап Генерації Mesh
            sw.Restart();
            var meshBuilder = new MeshBuilder();
            var mesh = meshBuilder.Build(octree);
            sw.Stop();
            totalMeshTime += sw.Elapsed.TotalMilliseconds;

            // 4. Етап Створення об'єкта Chunk
            sw.Restart();
            var chunk = new Chunk(position);
            chunk.Octree = octree;
            chunk.Mesh = mesh;
            sw.Stop();
            totalChunkTime += sw.Elapsed.TotalMilliseconds;
        }

        // Вивід результатів
        Console.WriteLine("--- Результати (Середній час на етап) ---");
        PrintResult("Generator Setup", totalGenTime, iterations);
        PrintResult("Octree Build   ", totalOctreeTime, iterations);
        PrintResult("Mesh Building  ", totalMeshTime, iterations);
        PrintResult("Chunk Instance ", totalChunkTime, iterations);

        double grandTotal = totalGenTime + totalOctreeTime + totalMeshTime + totalChunkTime;
        double grandAverage = grandTotal / iterations;
        Console.WriteLine($"Загальний час: {grandTotal:N2} ms");
        Console.WriteLine($"Загальний середній час: {grandAverage:F4} ms");
    }

    private static void PrintResult(string label, double totalMs, int count)
    {
        double average = totalMs / count;
        // Використовуємо F4 для точності до 4 знаків після коми
        Console.WriteLine($"{label}: {average:F3} ms");
    }
}