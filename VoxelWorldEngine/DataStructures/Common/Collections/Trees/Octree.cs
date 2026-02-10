using VoxelWorldEngine.DataStructures.Common.Collections.Trees.LinearImplementation;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.DataStructures.Common.Collections.Trees;

public class Octree<T> : IOctree<T>
    where T : struct
{
    private LinearOctree<T> _linearOctree;
    



}