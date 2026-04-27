using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Content.Commands;

public readonly record struct CubeArgs(int Radius, BlockId Block) : IShapeFactoryArgs;

public class CubeCommandFactory : ShapeCommandFactoryBase<CubeArgs>
{
    protected override CachedShapeData CreateShape(CubeArgs args)
    {
        var radius = args.Radius;
        var diameter = radius * 2 + 1;
        var size = new Vector3Int(diameter, diameter, diameter);
        var rootOffset = new Vector3Int(radius, radius, radius);
        var fillMask = new bool[diameter * diameter * diameter];
        Array.Fill(fillMask, true);

        return new CachedShapeData(size, rootOffset, fillMask);
    }
}