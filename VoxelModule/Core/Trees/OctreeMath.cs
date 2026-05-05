using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Trees;


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
}