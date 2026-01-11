using System.Diagnostics;
using System.Numerics;
using DotnetNoise;

namespace ConsoleApp1;

internal class Program
{
    private static readonly FastNoise _noise = new();


    private static void Main(string[] args)
    {
  
        var octant = 4;
        var mask = 1 << octant;
        var r = (byte)~mask;

        Console.WriteLine($"for {octant}");
        Console.WriteLine($"  {mask:b8}");
        Console.WriteLine($"  {r:b8}");
        

        
    }

  
}