using VoxelModule.Engine.Commands;
using VoxelModule.Engine.Octree;

namespace VoxelModule.Content.Commands;

public interface IShapeFactoryArgs
{
    int Radius { get; }
    BlockId Block { get; }
    ModifyMode Mode { get; }
}