namespace VoxelWorldEngine.DataStructures.LinearOctree;

/// <summary>
///     Class for working with octree data structure. Provides methods for calculating octant index and position the octant. Octant indices are 0-7 in order 0b_ZYX.
/// </summary>
public static class OctreeMath
{
    /// <summary>
    ///     Calculate octant index for position in octree.
    /// </summary>
    /// <param name="position">Local position in octree 0-size.</param>
    /// <param name="size">Size of octree. Power of 2.</param>
    /// <returns>0-7 octant index.</returns>
    public static int GetOctant(Vector3Int.Vector3Int position, int size)
    {
        var halfSize = size >> 1;
        var octant = 0;
        
        if (position.X >= halfSize) octant |= 1;
        if (position.Y >= halfSize) octant |= 2;
        if (position.Z >= halfSize) octant |= 4;
        
        return octant;
    }
    
    /// <summary>
    ///     Calculate position of 0's octant corner.
    /// </summary>
    /// <param name="octant">0-7 octant index</param>
    /// <param name="size">Size of octree. Power of 2.</param>
    /// <returns>Position</returns>
    public static Vector3Int.Vector3Int GetOctantCorner(int octant, int size)
    {
        var halfSize = size >> 1;
        
        return new Vector3Int.Vector3Int(
            (octant & 1) != 0 ? halfSize : 0, // x
            (octant & 2) != 0 ? halfSize : 0, // y
            (octant & 4) != 0 ? halfSize : 0  // z
        );
    }


    /// <summary>
    ///     Calculate the way to position in octree.
    /// </summary>
    /// <param name="position">Local position in octree 0-size.</param>
    /// <param name="size">Size of octree. Power of 2.</param>
    /// <returns>Array of octant indices way to position</returns>
    public static int[] FindWayTo(Vector3Int.Vector3Int position, int size)
    {
        var depth = int.Log2(size);
        var way = new int[depth];

        for (int i = 0; i < depth; i++)
        {
            var octant = GetOctant(position, size);
            way[i] = octant;
            var newOrigin = GetOctantCorner(octant, size);
            position -= newOrigin;
            
            size >>= 1;
        }
        return way;
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