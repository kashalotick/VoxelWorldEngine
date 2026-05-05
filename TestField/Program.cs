using System.Diagnostics;
using VoxelModule;
using VoxelModule.Core;
using VoxelModule.Engine;
using VoxelModule.Engine.Builders;
using VoxelModule.Engine.Octree;
using Chunk = VoxelModule.Engine.Chunks.Chunk;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace TestField;

class Program
{
    static void Main(string[] args)
    {
        var seed = 151234;
        
        // Можна вибрати один або обидва методи тестування
        // ProfileBuildMethod(seed, new Vector3Int(0, 0, 0), 100);
        
        ProfileBuildAreaMethod(seed, new Vector3Int(-5, 0, -5), new Vector3Int(5, 1, 5));
        
        Counter.Display();
    }

    private static void ProfileBuildMethod(int seed, Vector3Int position, int iterations = 200)
    {
        double totalGenTime = 0;
        double totalOctreeTime = 0;
        double totalOldMeshTime = 0;
        double totalNewMeshTime = 0;
        double totalChunkTime = 0;

        Stopwatch sw = new Stopwatch();

        Console.WriteLine($"Початок профілювання: {iterations} ітерацій...");

        for (int i = 0; i < iterations; i++)
        {
            Counter.Increment(CounterType.VoxelOctreeBuild);

            // 1. Етап Генератора
            sw.Restart();
            IGenerator generator = new SurfaceGenerator(seed, Chunk.ChunkToGlobal(position));
            sw.Stop();
            totalGenTime += sw.Elapsed.TotalMilliseconds;

            // 2. Етап Побудови Octree
            sw.Restart();
            var octree = new VoxelOctree();
            octree.Build(generator);
            sw.Stop();
            totalOctreeTime += sw.Elapsed.TotalMilliseconds;

            // 3a. Етап Генерації ChunkMesh (Старий)
            sw.Restart();
            // var oldMeshBuilder = new OldMeshBuilder();
            // var oldMesh = oldMeshBuilder.Build(octree);
            sw.Stop();
            totalOldMeshTime += sw.Elapsed.TotalMilliseconds;

            // 3b. Етап Генерації ChunkMesh (Новий)
            sw.Restart();
            var newMeshBuilder = new MeshBuilder();
            var newMesh = newMeshBuilder.Build(octree);
            sw.Stop();
            totalNewMeshTime += sw.Elapsed.TotalMilliseconds;

            // 4. Етап Створення об'єкта Chunk
            sw.Restart();
            var chunk = new Chunk(position);
            chunk.Octree = octree;
            chunk.ChunkMesh = newMesh; // Використовуємо новий меш
            sw.Stop();
            totalChunkTime += sw.Elapsed.TotalMilliseconds;
        }

        PrintFinalResults(iterations, totalGenTime, totalOctreeTime, totalOldMeshTime, totalNewMeshTime, totalChunkTime);
    }

    private static void ProfileBuildAreaMethod(int seed, Vector3Int min, Vector3Int max)
    {
        double totalGenTime = 0;
        double totalOctreeTime = 0;
        double totalOldMeshTime = 0;
        double totalNewMeshTime = 0;
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

            sw.Restart();
            IGenerator generator = new SurfaceGenerator(seed, Chunk.ChunkToGlobal(position));
            sw.Stop();
            totalGenTime += sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            var octree = new VoxelOctree();
            octree.Build(generator);
            sw.Stop();
            totalOctreeTime += sw.Elapsed.TotalMilliseconds;

            // Old ChunkMesh Builder
            sw.Restart();
            // var oldMeshBuilder = new OldMeshBuilder();
            // oldMeshBuilder.Build(octree);
            sw.Stop();
            totalOldMeshTime += sw.Elapsed.TotalMilliseconds;

            // New ChunkMesh Builder
            sw.Restart();
            var newMeshBuilder = new MeshBuilder();
            var newMesh = newMeshBuilder.Build(octree);
            sw.Stop();
            totalNewMeshTime += sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            var chunk = new Chunk(position);
            chunk.Octree = octree;
            chunk.ChunkMesh = newMesh;
            sw.Stop();
            totalChunkTime += sw.Elapsed.TotalMilliseconds;
        }

        PrintFinalResults(iterations, totalGenTime, totalOctreeTime, totalOldMeshTime, totalNewMeshTime, totalChunkTime);
    }

    private static void PrintFinalResults(int iterations, double totalGen, double totalOct, double totalOldM, double totalNewM, double totalChunk)
    {
        Console.WriteLine("\n--- Результати (Середній час на етап) ---");
        PrintResult("Generator Setup ", totalGen, iterations);
        PrintResult("Octree Build    ", totalOct, iterations);
        PrintResult("Old ChunkMesh Builder", totalOldM, iterations);
        PrintResult("New ChunkMesh Builder", totalNewM, iterations);
        PrintResult("Chunk Instance  ", totalChunk, iterations);

        Console.WriteLine("-----------------------------------------");
        double diff = totalOldM - totalNewM;
        string comparison = diff > 0 ? "швидше" : "повільніше";
        double percent = (Math.Abs(diff) / totalOldM) * 100;
        
        Console.WriteLine($"Новий білдер {comparison} за старий на {Math.Abs(diff/iterations):F4} ms ({percent:F1}%)");
        Console.WriteLine($"Загальний середній час (з новим): {(totalGen + totalOct + totalNewM + totalChunk) / iterations:F4} ms");
    }

    private static void PrintResult(string label, double totalMs, int count)
    {
        double average = totalMs / count;
        Console.WriteLine($"{label}: {average:F4} ms");
    }
}