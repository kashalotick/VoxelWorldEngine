using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Content.Commands;

public readonly record struct SphereArgs(int Radius, BlockId Block) : IShapeFactoryArgs;

public class SphereCommandFactory : ShapeCommandFactoryBase<SphereArgs>
{

    protected override CachedShapeData CreateShape(SphereArgs args)
    {
        var radius = args.Radius;
        var diameter = radius * 2 + 1;
        var size = new Vector3Int(diameter, diameter, diameter);
        var rootOffset = new Vector3Int(radius, radius, radius);
        var fillMask = new bool[diameter * diameter * diameter];
        var radiusSquared = radius * radius;

        for (int y = 0; y < diameter; y++)
        for (int z = 0; z < diameter; z++)
        for (int x = 0; x < diameter; x++)
        {
            var dx = x - radius;
            var dy = y - radius;
            var dz = z - radius;
            fillMask[x + y * diameter + z * diameter * diameter] = dx * dx + dy * dy + dz * dz <= radiusSquared;
        }

        return new CachedShapeData(size, rootOffset, fillMask);
    }
}