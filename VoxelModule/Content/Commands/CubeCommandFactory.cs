using VoxelModule.Engine.Commands;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Content.Commands;

public readonly record struct CubeArgs(int Radius, BlockId Block, ModifyMode Mode) : IShapeFactoryArgs;

public class CubeCommandFactory : ShapeCommandFactoryBase<CubeArgs>
{
    protected override CachedShapeData CreateShape(CubeArgs args)
    {
        var radius = args.Radius - 1;
        var diameter = radius * 2 + 1;
        var size = new Vector3Int(diameter, diameter, diameter);
        var rootOffset = new Vector3Int(radius, radius, radius);
        var fillMask = new bool[diameter * diameter * diameter];
        Array.Fill(fillMask, true);

        return new CachedShapeData(size, rootOffset, fillMask);
    }
}