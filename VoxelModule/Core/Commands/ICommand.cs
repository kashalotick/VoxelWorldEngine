using VoxelModule.DataStructures.Common.Collections.Trees;
using VoxelModule.DataStructures.Common.Structures.Vectors;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.Core.Commands;

public interface ICommand
{
    void Execute();
}