using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

public class VoxelOctree : Octree<Voxel>
{
    public void Build(IGenerator generator)
    {
        var stack = new Stack<OctreeNode>();
        var root = Root();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            
            if (generator.IsUniform(node.Min, node.Max) || node.Depth == MaxDepth )
            {
                node.Data = generator.Approximate(node.Min, node.Max);
            }
            else
            {
                node.Split();
                for (int i = 0; i < 8; i++)
                {
                    var child = node.GetChild(i);
                    stack.Push(child);
                }
            }
        }
    }

}