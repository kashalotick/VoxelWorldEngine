using System.Numerics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

public class VoxelOctree : Octree<Voxel>, IVoxelOctree, IWorldRegion
{

    
    public void Build(IGenerator generator)
    {
        BuildRecursive(generator, Root());
        // Console.WriteLine($"Split: {_splitCount}, Merge: {_mergeCount}");
    }

    private void BuildRecursive(IGenerator generator, OctreeNode node)
    {
        if (generator.IsUniform(node.MinIndex, node.MaxIndex) || node.Depth == MaxDepth)
        {
            node.Data = generator.Approximate(node.MinIndex, node.MaxIndex);
            return;
        }

        node.Split();
        var shouldMerge = true;
        Voxel? childVoxel = null;
        for (int i = 0; i < 8; i++)
        {
            var child = node.GetChild(i);
            BuildRecursive(generator, child);
            
            if (!child.IsLeaf)
            {
                shouldMerge = false;
            } else if (child.Data.BlockId == BlockId.Mixed)
            {
                shouldMerge = false;
            }
            else
            {
                if (childVoxel == null)
                {
                    childVoxel = child.Data;
                } else if (!Equals(childVoxel, child.Data))
                {
                    shouldMerge = false;
                }
            }

        }
        if (shouldMerge)
        {
            node.Data = childVoxel ??  new Voxel();
            node.Merge();
        }
    }
    

    public RayHit Raycast(Ray ray)
    {
        var root = Root();

        // Межі кореневого вузла в float
        var minF = new Vector3(root.MinIndex.X, root.MinIndex.Y, root.MinIndex.Z);
        var maxF = new Vector3(root.MaxIndex.X + 1, root.MaxIndex.Y + 1, root.MaxIndex.Z + 1);

        // Знаходимо точку входу/виходу променя в AABB кореня
        if (!RaycastUtils.IntersectAABB(ray, minF, maxF, out float tMin, out float tMax, out Vector3 normal))
            return RayHit.NoHit;

        tMin = MathF.Max(tMin, 0f);
        tMax = MathF.Min(tMax, ray.Length);
        if (tMin > tMax)
            return RayHit.NoHit;


        return RaycastNode(root, ray, tMin, tMax, normal);
    }

    private RayHit RaycastNode(OctreeNode node, Ray ray, float tMin, float tMax, Vector3 normal)
    {

        if (node.IsLeaf)
        {
            var voxel = node.Data;
            if (voxel.IsAir)
                return RayHit.NoHit;

            return new RayHit
            {
                HitIn  = ray.Origin + ray.Direction * tMin,
                HitOut = ray.Origin + ray.Direction * MathF.Min(tMax, ray.Length),
                Voxel  = voxel,
                HitFaceNormal = normal   // <-- нормаль грані входу
            };
        }

        Span<(int octant, float t0, float t1, Vector3 n)> hits =
            stackalloc (int, float, float, Vector3)[8];
        int hitCount = 0;

        for (int octant = 0; octant < 8; octant++)
        {
            var child = node.GetChild(octant);
            var cMin = new Vector3(child.MinIndex.X, child.MinIndex.Y, child.MinIndex.Z);
            var cMax = new Vector3(child.MaxIndex.X + 1, child.MaxIndex.Y + 1, child.MaxIndex.Z + 1);

            if (!RaycastUtils.IntersectAABB(ray, cMin, cMax, out float ct0, out float ct1, out Vector3 childNormal))
                continue;

            ct0 = MathF.Max(ct0, tMin);
            ct1 = MathF.Min(ct1, tMax);

            if (ct0 <= ct1 && ct0 <= ray.Length)
                hits[hitCount++] = (octant, ct0, ct1, childNormal);
        }

        // insertion sort
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

        for (int i = 0; i < hitCount; i++)
        {
            var (octant, ct0, ct1, childNormal) = hits[i];
            var child = node.GetChild(octant);
            var result = RaycastNode(child, ray, ct0, ct1, childNormal);  // <-- передаємо нормаль
            if (result.IsHit)
                return result;
        }

        return RayHit.NoHit;
    }

    public bool PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId)
    {
        return SetData(voxelPositionIndex, new Voxel(blockId), null);
    }
}