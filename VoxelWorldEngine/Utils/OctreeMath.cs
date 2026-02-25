using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Utils;


public static class OctreeMath
{

    public static int GetOctant(Vector3Int position, int size)
    {
        var halfSize = size >> 1;
        var octant = 0;
        
        if (position.X >= halfSize) octant |= 1;
        if (position.Y >= halfSize) octant |= 2;
        if (position.Z >= halfSize) octant |= 4;
        
        return octant;
    }
    

    public static Vector3Int GetOctantCorner(int octant, int size)
    {
        var halfSize = size >> 1;
        
        return new Vector3Int(
            (octant & 1) != 0 ? halfSize : 0, // x
            (octant & 2) != 0 ? halfSize : 0, // y
            (octant & 4) != 0 ? halfSize : 0  // z
        );
    }

    
    public static void FindWayTo(Vector3Int position, int size, Span<int> way)
    {
        var depth = int.Log2(size);

        for (int i = 0; i < depth; i++)
        {
            var octant = GetOctant(position, size);
            way[i] = octant;
            var newOrigin = GetOctantCorner(octant, size);
            position -= newOrigin;
            
            size >>= 1;
        }
    }
    
    
    // public static int FindWayCodeTo(Vector3Int.Vector3Int position, int size)
    // {
    //     var depth = int.Log2(size);
    //     var way = 0;
    //
    //     for (int i = 0; i < depth; i++)
    //     {
    //         var octant = GetOctant(position, size);
    //         way <<= 3;
    //         way |= octant;
    //         var newOrigin = GetOctantCorner(octant, size);
    //         position -= newOrigin;
    //         
    //         size >>= 1;
    //     }
    //     return way;
    // }
    //
    // public static void TestWayCode(Vector3Int.Vector3Int position, int size)
    // {
    //     var wayCode = FindWayCodeTo(position, size);
    //     var depth = int.Log2(size);
    //
    //     for (int i = 0; i < depth; i++)
    //     {
    //         var shift = (depth - i - 1) * 3;
    //         var octant = (wayCode >> shift) & 0b_111;
    //         // process octant
    //     }
    // }
    
}