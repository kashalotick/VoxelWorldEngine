using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Content.Commands;


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