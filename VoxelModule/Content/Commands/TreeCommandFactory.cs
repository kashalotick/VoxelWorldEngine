using VoxelModule.Core;
using VoxelModule.Engine;
using VoxelModule.Engine.Commands;
using VoxelModule.Engine.Octree;
using VoxelModule.Utils;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Content.Commands;


public readonly record struct TreeArgs(int Seed);


public class TreeCommandFactory : ICommandFactory<TreeArgs>

{
    private readonly BlockId[,,] _data =
    {
        {
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Wood, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
        },
        {
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Wood, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
        },
        {
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Wood, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
        },
        {
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Wood, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
        },
        {
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Wood, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
            { BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves },
        },
        {
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Void },
            { BlockId.Void, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Void },
            { BlockId.Void, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
        },
        {
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Leaves, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Leaves, BlockId.Leaves, BlockId.Leaves, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Leaves, BlockId.Void, BlockId.Void },
            { BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void, BlockId.Void },
        },
    };

    private Vector3Int _rootOffset = new(2, 0, 2);

    private readonly Vector3Int _dataSize;
    private readonly Voxel[] _voxelData;

    public TreeCommandFactory()
    {
        _dataSize = ArrayHelper.GetArraySize(_data);
        var flatten = ArrayHelper.Flatten3DArray(_data);
        _voxelData = flatten.Select(blockId =>  new Voxel(blockId)).ToArray();
    }


    public ModifyRegionCommand GetCommand(IWorldRegion region, Vector3Int position, TreeArgs args)
    {
        var globalPosition = position - _rootOffset;

        var command = new ModifyRegionCommand(region, globalPosition, _dataSize, _voxelData, ModifyMode.ReplaceAir);
        return command;
    }
}