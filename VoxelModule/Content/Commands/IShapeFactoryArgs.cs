using VoxelModule.Core.Commands;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Content.Commands;

public interface IShapeFactoryArgs
{
    int Radius { get; }
    BlockId Block { get; }
    ModifyMode Mode { get; }
}