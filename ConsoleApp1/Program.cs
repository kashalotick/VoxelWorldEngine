using DotnetNoise;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Generator;
using VoxelWorldEngine.Utils;
using VoxelWorldEngine.Utils.Vector3Int;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        
        var s = 2 >> 3;

        var sfg = new SimplexSurfaceGenerator();
        
        var top = new Vector3Int(4, 10, -142);
        var middle = new Vector3Int(4, 10, 120);
        var bottom = new Vector3Int(4, 10, 5);



        Console.WriteLine(sfg.GetValue(top));
        Console.WriteLine(sfg.GetValue(middle));
        Console.WriteLine(sfg.GetValue(bottom));

        Console.WriteLine(sfg.GetMaximum(top, middle));
        Console.WriteLine(sfg.GetMinimum(top, middle));



    }
}