using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace VoxelWorldEngine.Core.Builders;

public class MeshBuilder
{
    private readonly VoxelOctree _octree;

    private bool?[,,] _isEmptyCache;

    public MeshBuilder(VoxelOctree octree)
    {
        _octree = octree;
    }


    public MeshData Build()
    {
        _isEmptyCache = new bool?[_octree.Size, _octree.Size, _octree.Size];

        var mesh = new MeshData();
        var stack = new Stack<VoxelOctree.OctreeNode>();

        var counter = 0;
        stack.Push(_octree.Root());

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.IsLeaf)
            {
                if (node.Data.IsEmpty) continue;

                AddFaces(node, mesh);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    stack.Push(node.GetChild(i));
                    counter++;
                }
            }
        }

        _isEmptyCache = new bool?[0,0,0];

        return mesh;
    }


    private void AddFaces(VoxelOctree.OctreeNode node, MeshData mesh)
    {
        for (int i = 0; i < 6; i++)
        {
            var normal = Normals[i];

            // if (FaceCulling(node, normal)) continue;

            AddFace(node, normal, mesh);
        }
    }

    private bool FaceCulling(VoxelOctree.OctreeNode node, Vector3Int normal)
    {
        if (normal.X != 0)
        {
            int x = normal.X > 0 ? node.Max.X + 1 : node.Min.X - 1;

            for (int y = node.Min.Y; y <= node.Max.Y; y++)
            for (int z = node.Min.Z; z <= node.Max.Z; z++)
            {
                var pos = new Vector3Int(x, y, z);

                if (IsEmpty(pos)) return false;
            }

            return true;
        }

        if (normal.Y != 0)
        {
            int y = normal.Y > 0 ? node.Max.Y + 1 : node.Min.Y - 1;

            for (int x = node.Min.X; x <= node.Max.X; x++)
            for (int z = node.Min.Z; z <= node.Max.Z; z++)
            {
                var pos = new Vector3Int(x, y, z);

                if (IsEmpty(pos)) return false;
            }

            return true;
        }

        // Z
        int zFixed = normal.Z > 0 ? node.Max.Z + 1 : node.Min.Z - 1;

        for (int x = node.Min.X; x <= node.Max.X; x++)
        for (int y = node.Min.Y; y <= node.Max.Y; y++)
        {
            var pos = new Vector3Int(x, y, zFixed);

            if (IsEmpty(pos)) return false;
        }

        return true;
    }

    private bool IsEmpty(Vector3Int pos)
    {
        if (!pos.IsInBounds(_octree.Root().Size))
        {
            return true;
        }

        var cached = _isEmptyCache[pos.X, pos.Y, pos.Z];
        if (cached.HasValue)
            return cached.Value;
    
        var isEmpty = _octree.GetData(pos).IsEmpty;
        _isEmptyCache[pos.X, pos.Y, pos.Z] = isEmpty;
        return isEmpty;

    }


    private void AddFace(VoxelOctree.OctreeNode node, Vector3Int normal, MeshData mesh)
    {
        var facePosition = (Vector3)node.Min;
        var faceNormal = (Vector3)normal;
        var vCount = (uint)mesh.Vertices.Count;
        var normalIndex = GetFaceIndex(normal);

        for (int i = 0; i < 4; i++)
        {
            var offset = FaceVertices[normalIndex][i];
            var vertex = new ChunkVertex
            {
                Position = facePosition + node.Size * offset,
                Normal = faceNormal,
                Uv = FaceUVs[i] * node.Size,
                BlockId = node.Data.BlockId
            };
            mesh.Vertices.Add(vertex);
        }

        mesh.Indices.Add(vCount + 0);
        mesh.Indices.Add(vCount + 1);
        mesh.Indices.Add(vCount + 2);
        mesh.Indices.Add(vCount + 2);
        mesh.Indices.Add(vCount + 3);
        mesh.Indices.Add(vCount + 0);
    }

    private static readonly Vector2[] FaceUVs =
    [
        new(0, 0), // Нижній лівий
        new(1, 0), // Нижній правий
        new(1, 1), // Верхній правий
        new(0, 1) // Верхній лівий
    ];

    private static readonly Vector3Int[] Normals =
    [
        Vector3Int.UnitY,
        -Vector3Int.UnitY,
        Vector3Int.UnitZ,
        -Vector3Int.UnitZ,
        Vector3Int.UnitX,
        -Vector3Int.UnitX
    ];

    private static readonly Vector3[][] FaceVertices =
    [
        // Top (+Y)
        [new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0)],
        // Bottom (-Y)
        [new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)],
        // Front (+Z)
        [new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)],
        // Back (-Z)
        [new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)],
        // Right (+X)
        [new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1)],
        // Left (-X)
        [new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0)]
    ];


    private int GetFaceIndex(Vector3Int normal)
    {
        if (normal.Y > 0) return 0; // Top
        if (normal.Y < 0) return 1; // Bottom
        if (normal.Z > 0) return 2; // Front
        if (normal.Z < 0) return 3; // Back
        if (normal.X > 0) return 4; // Right
        return 5; // Left
    }
}