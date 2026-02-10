using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

public class VoxelOctree : Octree<Voxel>
{
    public void Build(IGenerator generator)
    {
        var stack = new Stack<IOctreeNode<Voxel>>();
        var root = Root();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            
            if (generator.IsUniform(root.Min, root.Max))
            {
                root.Data = generator.Approximate(root.Min, root.Max);
            }
            else
            {
                root.Split();
                foreach (var child in root.Children)
                {
                    stack.Push(child);
                }
            }
        }
    }

}

public interface IGenerator
{
    Voxel Approximate(Vector3Int min, Vector3Int max);
    bool IsUniform(Vector3Int min, Vector3Int max);
}