using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Content.Commands;

public interface ICommandFactory<TArgs>
    where TArgs : struct
{
    ModifyRegionCommand GetCommand(IWorldRegion region, Vector3Int position, TArgs args);
}
