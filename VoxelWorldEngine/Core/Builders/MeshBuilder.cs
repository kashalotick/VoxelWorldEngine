using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace VoxelWorldEngine.Core.Builders;

public class MeshBuilder
{
    private VoxelOctree _octree;

    private bool?[,,] _isEmptyCache = new bool?[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkSize];

    private Stack<VoxelOctree.OctreeNode> _stack = new();

    private List<uint> _indices = new (4096);
    private List<ChunkVertex> _vertices = new (4096);


    public MeshData Build(VoxelOctree octree)
    {
        _octree = octree;

        
        _stack.Clear();
        var stack = _stack;

        var counter = 0;
        stack.Push(_octree.Root());

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.IsLeaf)
            {
                if (node.Data.IsEmpty) continue;

                AddFaces(node);
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

        var mesh = new MeshData(_indices.ToArray(), _vertices.ToArray());
        _indices.Clear();
        _vertices.Clear();
        Array.Clear(_isEmptyCache, 0, _isEmptyCache.Length);

        return mesh;
    }


    private void AddFaces(VoxelOctree.OctreeNode node)
    {
        for (int i = 0; i < 6; i++)
        {
            var normal = Normals[i];

            if (FaceCulling(node, normal)) continue;

            AddFace(node, normal);
        }
    }

    private bool FaceCulling(VoxelOctree.OctreeNode node, Vector3Int normal)
    {
        if (normal.X != 0)
        {
            int x = normal.X > 0 ? node.MaxIndex.X + 1 : node.MinIndex.X - 1;

            for (int y = node.MinIndex.Y; y <= node.MaxIndex.Y; y++)
            for (int z = node.MinIndex.Z; z <= node.MaxIndex.Z; z++)
            {
                var pos = new Vector3Int(x, y, z);

                if (IsEmpty(pos)) return false;
            }

            return true;
        }

        if (normal.Y != 0)
        {
            int y = normal.Y > 0 ? node.MaxIndex.Y + 1 : node.MinIndex.Y - 1;

            for (int x = node.MinIndex.X; x <= node.MaxIndex.X; x++)
            for (int z = node.MinIndex.Z; z <= node.MaxIndex.Z; z++)
            {
                var pos = new Vector3Int(x, y, z);

                if (IsEmpty(pos)) return false;
            }

            return true;
        }

        // Z
        int zFixed = normal.Z > 0 ? node.MaxIndex.Z + 1 : node.MinIndex.Z - 1;

        for (int x = node.MinIndex.X; x <= node.MaxIndex.X; x++)
        for (int y = node.MinIndex.Y; y <= node.MaxIndex.Y; y++)
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


    private void AddFace(VoxelOctree.OctreeNode node, Vector3Int normal)
    {
        var facePosition = (Vector3)node.MinIndex;
        var faceNormal = (Vector3)normal;
        var vCount = (uint)_vertices.Count;
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
            _vertices.Add(vertex);
        }

        _indices.Add(vCount + 0);
        _indices.Add(vCount + 1);
        _indices.Add(vCount + 2);
        _indices.Add(vCount + 2);
        _indices.Add(vCount + 3);
        _indices.Add(vCount + 0);
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