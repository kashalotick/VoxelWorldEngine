using System.Numerics;
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
        var root = Root();
    
        // Межі кореневого вузла в float
        var minF = new Vector3(root.Min.X, root.Min.Y, root.Min.Z);
        var maxF = new Vector3(root.Max.X + 1, root.Max.Y + 1, root.Max.Z + 1);
    
        // Знаходимо точку входу/виходу променя в AABB кореня
        if (!Raycaster.IntersectAABB(ray, minF, maxF, out float tMin, out float tMax))
            return new RayHit { Voxel = Voxel.Empty };

        tMin = MathF.Max(tMin, 0f);
        if (tMin > tMax)
            return new RayHit { Voxel = Voxel.Empty };

        return RaycastNode(root, ray, tMin, tMax);
    }
    private RayHit RaycastNode(OctreeNode node, Ray ray, float tMin, float tMax)
    {
        if (tMin > ray.Length)
            return new RayHit { Voxel = Voxel.Empty };

        // Листовий вузол — повертаємо результат
        if (node.IsLeaf)
        {
            var voxel = node.Data;
            if (voxel.IsEmpty)
                return new RayHit { Voxel = Voxel.Empty };

            return new RayHit
            {
                HitIn  = ray.Origin + ray.Direction * tMin,
                HitOut = ray.Origin + ray.Direction * MathF.Min(tMax, ray.Length),
                Voxel  = voxel
            };
        }

        // Сортуємо дочірні вузли за відстанню входу
        Span<(int octant, float t0, float t1)> hits = stackalloc (int, float, float)[8];
        int hitCount = 0;

        for (int octant = 0; octant < 8; octant++)
        {
            var child = node.GetChild(octant);
            var cMin = new Vector3(child.Min.X, child.Min.Y, child.Min.Z);
            var cMax = new Vector3(child.Max.X + 1, child.Max.Y + 1, child.Max.Z + 1);

            if (!Raycaster.IntersectAABB(ray, cMin, cMax, out float ct0, out float ct1))
                continue;

            ct0 = MathF.Max(ct0, tMin);
            ct1 = MathF.Min(ct1, tMax);

            if (ct0 <= ct1 && ct0 <= ray.Length)
                hits[hitCount++] = (octant, ct0, ct1);
        }

        // Сортування по t0 (insertion sort, бо масив малий — max 8)
        for (int i = 1; i < hitCount; i++)
        {
            var cur = hits[i];
            int j = i - 1;
            while (j >= 0 && hits[j].t0 > cur.t0)
            {
                hits[j + 1] = hits[j];
                j--;
            }
            hits[j + 1] = cur;
        }

        // Рекурсивно перевіряємо в порядку зростання відстані
        for (int i = 0; i < hitCount; i++)
        {
            var (octant, ct0, ct1) = hits[i];
            var child = node.GetChild(octant);
            var result = RaycastNode(child, ray, ct0, ct1);
            if (result.IsHit)
                return result;
        }

        return new RayHit { Voxel = Voxel.Empty };
    }
}