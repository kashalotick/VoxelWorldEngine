using FastNoiseOO;
using VoxelWorldEngine.Core.Octrees;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {

        // var octree = new SparseOctree();
        //
        // Console.WriteLine(octree.Size);


        var noise = new FastNoiseOO.Generators.SineWave();
        var t = noise.GenUniformGrid2D(-100, -100, 100, 100, 0.1f, 120, out var minMax);
        Console.WriteLine(minMax);


    }
}