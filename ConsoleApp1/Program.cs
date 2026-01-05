using VoxelWorldEngine.Core;
using VoxelWorldEngine.Utils;
using VoxelWorldEngine.Utils.Vector3Int;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        var mins = new Seed(100_000_000_000_000_000L);
        
        var seed = new Seed(284_014_432_143_005_581L);
        var world = new World(seed);
        
        
        var observer = new Vector3Int(0);
        
        world.RecalculateChunksAround(observer);
        

        Console.WriteLine(world.Chunks.Count);
        Console.WriteLine(world.Chunks[observer].Voxels.Length);


        var serializer = new WorldSerializer();
        serializer.Save(world, observer);
    }

    static void GenerateChunks()
    {
        
    }
}