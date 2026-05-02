using VoxelModule.Core;
using VoxelModule.Core.Commands;
using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace VoxelModule.Content.Commands;

public interface ICommandFactory<TArgs>
    where TArgs : struct
{
    ModifyRegionCommand GetCommand(IWorldRegion region, Vector3Int position, TArgs args);
}
