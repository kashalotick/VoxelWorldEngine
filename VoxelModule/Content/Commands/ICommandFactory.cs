using VoxelModule.Core;
using VoxelModule.Engine;
using VoxelModule.Engine.Commands;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Content.Commands;

public interface ICommandFactory<TArgs>
    where TArgs : struct
{
    ModifyRegionCommand GetCommand(IWorldRegion region, Vector3Int position, TArgs args);
}
