using System.Numerics;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Mesh;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.Core.Builders;

public class MeshBuilder
{
    private Vector3Int[] _directions =
    [
        Vector3Int.UnitX, Vector3Int.UnitY, Vector3Int.UnitZ,
        -Vector3Int.UnitX, -Vector3Int.UnitY, -Vector3Int.UnitZ
    ];


    public Mesh Build(LinearOctree octree)
    {
        var queue = new Queue<MeshNode>();
        var mesh = new Mesh();

        var rootMeshNode = new MeshNode(octree.RootIndex, LinearOctree.Size, Vector3Int.Zero);
        queue.Enqueue(rootMeshNode);

        var cycleCount = 0;
        while (queue.Count > 0)
        {
            cycleCount++;

            var meshNode = queue.Dequeue();
            var octreeNode = octree.GetNode(meshNode.Index);

            if (octreeNode.IsLeaf)
            {
                ProcessLeaf(meshNode, octree, mesh);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    var childIndex = octreeNode.GetChildIndex(i);
                    var childSize = meshNode.Size >> 1;
                    var localOffset = OctreeMath.GetOctantCorner(i, meshNode.Size);
                    var position = meshNode.Position + localOffset;

                    var childMeshNode = new MeshNode(childIndex, childSize, position);
                    queue.Enqueue(childMeshNode);
                }
            }
        }

        return mesh;
    }


    private void ProcessLeaf(MeshNode node, LinearOctree octree, Mesh mesh)
    {
        var octreeNode = octree.GetNode(node.Index);
        if (octreeNode.IsAir) return;

        foreach (var direction in _directions)
        {
            if (IsFaceVisible(node, direction, octree))
            {
                mesh.AddFace(node.Position, direction, node.Size);
            }
        }
    }

    // TODO: test new impl; remake with bounds; impl bounds iterator in octree
    private bool IsFaceVisible(MeshNode node, Vector3Int direction, LinearOctree octree)
    {
        // Перевіряємо всю грань, а не одну точку
        var size = node.Size;
        // var halfSize = size >> 1;



        var newOrigin = (node.Position.ToVector3() + Vector3.One * (size - 1)) / 2;
        var transformMatrix = VectorHelper.GetTransformMatrix(newOrigin, direction.ToVector3());
        //
        // var minBound = new Vector3Int(0, 0, size);
        // var maxBound = new Vector3Int(size - 1, size - 1, size);
        // minBound = VectorHelper.Transform(minBound, transformMatrix);
        // maxBound = VectorHelper.Transform(maxBound, transformMatrix);
        // if (IsAirInArea(minBound, maxBound, octree))
        // {
        //     return true;
        // }
        //
        var localZ = Vector3Int.Dot(direction, Vector3Int.One) > 0 ? size : -1;
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        {
            var checkPos = new Vector3Int(x, y, localZ);
            checkPos = VectorHelper.Transform(checkPos, transformMatrix);
            if (IsPositionAir(checkPos, octree))
                return true; 
        }
        
        // // Визначаємо по якій осі йде напрямок
        // if (direction.X != 0)
        // {
        //     // Грань перпендикулярна X
        //     var checkX = node.Position.X + (direction.X > 0 ? size : -1);
        //
        //     for (int y = 0; y < size; y++)
        //     for (int z = 0; z < size; z++)
        //     {
        //         var checkPos = new Vector3Int(checkX, node.Position.Y + y, node.Position.Z + z);
        //         if (IsPositionAir(checkPos, octree))
        //             return true; // Хоч одна точка грані видима
        //     }
        // }
        // else if (direction.Y != 0)
        // {
        //     var checkY = node.Position.Y + (direction.Y > 0 ? size : -1);
        //
        //     for (int x = 0; x < size; x++)
        //     for (int z = 0; z < size; z++)
        //     {
        //         var checkPos = new Vector3Int(node.Position.X + x, checkY, node.Position.Z + z);
        //         if (IsPositionAir(checkPos, octree))
        //             return true;
        //     }
        // }
        // else if (direction.Z != 0)
        // {
        //     var checkZ = node.Position.Z + (direction.Z > 0 ? size : -1);
        //
        //     for (int x = 0; x < size; x++)
        //     for (int y = 0; y < size; y++)
        //     {
        //         var checkPos = new Vector3Int(node.Position.X + x, node.Position.Y + y, checkZ);
        //         if (IsPositionAir(checkPos, octree))
        //             return true;
        //     }
        // }

        return false; // Вся грань закрита
    }

    private bool IsPositionAir(Vector3Int position, LinearOctree octree)
    {
        if (!position.IsInBounds(LinearOctree.Size))
            return true;

        var node = octree.GetNode(position);
        return !node.IsSolid;
    }
}

public struct MeshNode
{
    public int Index;
    public int Size;
    public Vector3Int Position;

    public MeshNode(int index, int size, Vector3Int position)
    {
        Index = index;
        Size = size;
        Position = position;
    }
}