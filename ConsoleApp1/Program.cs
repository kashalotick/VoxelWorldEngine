using VoxelWorldEngine.Core;
using VoxelWorldEngine.Utils;
using VoxelWorldEngine.Utils.Vector3Int;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {

        byte b = 0b_0011_1101;

        Console.WriteLine(Byte.PopCount(b));
    }
}