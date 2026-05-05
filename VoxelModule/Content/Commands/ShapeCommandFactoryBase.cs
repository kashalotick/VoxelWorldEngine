using VoxelModule.Core;
using VoxelModule.Engine;
using VoxelModule.Engine.Commands;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Content.Commands;


public abstract class ShapeCommandFactoryBase<TArgs> : ICommandFactory<TArgs>
    where TArgs : struct, IShapeFactoryArgs
{
    private readonly Dictionary<int, CachedShapeData> _cache = new();

    public ModifyRegionCommand GetCommand(IWorldRegion region, Vector3Int position, TArgs args)
    {
        if (args.Radius < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(args), "Radius must be >= 0");
        }

        var shape = GetOrCreate(args.Radius, args);
        var data = BuildVoxelData(shape.FillMask, args.Block);
        var globalPosition = position - shape.RootOffset;

        return new ModifyRegionCommand(region, globalPosition, shape.Size, data, args.Mode);
    }
    
    protected abstract CachedShapeData CreateShape(TArgs args);

    protected readonly record struct CachedShapeData(Vector3Int Size, Vector3Int RootOffset, bool[] FillMask);

    private CachedShapeData GetOrCreate(int radius, TArgs args)
    {
        if (_cache.TryGetValue(radius, out var cached))
        {
            return cached;
        }

        var created = CreateShape(args);
        _cache[radius] = created;
        return created;
    }

    private static Voxel[] BuildVoxelData(bool[] fillMask, BlockId block)
    {
        var data = new Voxel[fillMask.Length];
        for (int i = 0; i < fillMask.Length; i++)
        {
            data[i] = new Voxel(fillMask[i] ? block : BlockId.Void);
        }

        return data;
    }
}

