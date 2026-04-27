using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Content.Commands;

public interface IShapeFactoryArgs
{
    int Radius { get; }
    BlockId Block { get; }
    ModifyMode Mode { get; }
}