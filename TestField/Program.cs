using System.Diagnostics;
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
        var seed = 151234;
        var iterations = 1000;
        // Console.WriteLine("\n-- Air chunk");
        // ProfileBuildMethod(seed, new Vector3Int(0, 1, 0), iterations);
        Console.WriteLine("\n-- Top surface chunk");
        ProfileBuildMethod(seed, new Vector3Int(0, 0, 0), iterations);
        Console.WriteLine("\n-- Down surface chunk");
        ProfileBuildMethod(seed, new Vector3Int(0, -1, 0), iterations);
        // Console.WriteLine("\n-- solid chunk");
        // ProfileBuildMethod(seed, new Vector3Int(0, -2, 0), iterations);
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
        // 1. Етап Генератора
        sw.Restart();
        IGenerator generator = new ProceduralGenerator(seed, ChunkMath.ChunkToGlobal(position));
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
        var meshBuilder = new MeshBuilder(octree);
        var mesh = meshBuilder.Build();
        sw.Stop();
        totalMeshTime += sw.Elapsed.TotalMilliseconds;

        // 4. Етап Створення об'єкта Chunk
        sw.Restart();
        var chunk = new Chunk(position, octree, mesh);
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



