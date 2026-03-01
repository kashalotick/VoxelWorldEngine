using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

public class VoxelOctree : Octree<Voxel>
{
    private static readonly ThreadLocal<Stack<OctreeNode>> _stackPool = 
        new(() => new Stack<OctreeNode>());

    private Stack<OctreeNode> _stack = new();
    public void Build(IGenerator generator)
    {
        var stack = _stackPool.Value;
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
        _stack.Clear();
    }
}