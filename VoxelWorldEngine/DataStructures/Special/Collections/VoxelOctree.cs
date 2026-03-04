using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

public class VoxelOctree : Octree<Voxel>, IRaycastable
{
    private static readonly ThreadLocal<Stack<OctreeNode>> _stackPool =
        new(() => new Stack<OctreeNode>());


    public void Build(IGenerator generator)
    {
        var stack = _stackPool.Value;
        stack.Clear();
        var root = Root();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (generator.IsUniform(node.Min, node.Max) || node.Depth == MaxDepth)
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

    private readonly (OctreeNode node, float tIn)[] _childBuffer = new (OctreeNode, float)[8];
    private readonly Stack<(OctreeNode node, float tIn)> _rayStack = new();

    public RayHit Raycast(Ray ray)
    {
        var stack = _rayStack;
        stack.Clear();
        var root = Root();
    
        if (!Raycaster.IntersectsAABB(ray, root.Min, root.Max, out float rootTIn, out float rootTOut))
            return new RayHit { Voxel = Voxel.Empty };
    
        stack.Push((root, rootTIn));

        while (stack.Count > 0)
        {
            var (node, tIn) = stack.Pop();

            if (node.IsLeaf)
            {
                if (node.Data.IsEmpty) continue;
                Raycaster.IntersectsAABB(ray, node.Min, node.Max, out float leafTIn, out float leafTOut);
    
                return new RayHit
                {
                    // ✅ Повертаємо глобальні координати, додаючи назад GlobalPosition
                    HitIn  = ray.Origin + ray.Direction * leafTIn,
                    HitOut = ray.Origin + ray.Direction * leafTOut,
                    Voxel  = node.Data
                };
            }

            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                var child = node.GetChild(i);
                if (Raycaster.IntersectsAABB(ray, child.Min, child.Max, out float childTIn, out float childTOut))
                    _childBuffer[count++] = (child, childTIn);
            }

            // Сортування по tIn
            for (int i = 1; i < count; i++)
            {
                var key = _childBuffer[i];
                int j = i - 1;
                while (j >= 0 && _childBuffer[j].tIn > key.tIn)
                    _childBuffer[j + 1] = _childBuffer[j--];
                _childBuffer[i] = key;
            }

            // Пушимо у зворотньому порядку
            for (int i = count - 1; i >= 0; i--)
                stack.Push(_childBuffer[i]);
        }

        return new RayHit { Voxel = Voxel.Empty };
    }
    
}