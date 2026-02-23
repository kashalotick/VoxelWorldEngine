using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.LinearOctree.Special.Structures.Vertices;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;

namespace VoxelWorldEngine.Core.Builders;

public class MeshBuilder
{
    private readonly VoxelOctree _octree;

    public MeshBuilder(VoxelOctree octree)
    {
        _octree = octree;
    }


    public MeshData Build()
    {
        var mesh = new MeshData();
        var stack = new Stack<VoxelOctree.OctreeNode>();


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
                }
            }
        }

        return mesh;
    }


    private void AddFaces(VoxelOctree.OctreeNode node, MeshData mesh)
    {
        for (int i = 0; i < 6; i++)
        {
            var normal = Normals[i];
        }
    }


    private void AddFace(Vector3Int position, Vector3Int normal, int size, MeshData mesh)
    {
        var facePosition = (Vector3)position;
        var faceNormal = (Vector3)normal;
        var faceColor = new Vector3(1, 2, 3);
        var vCount = (ushort)mesh.Vertices.Count;
        var normalIndex = GetFaceIndex(normal);

        for (int i = 0; i < 4; i++)
        {
            var offset = FaceVertices[normalIndex][i];
            var vertex = new Vertex
            {
                Position = facePosition + offset * size,
                Normal = faceNormal,
                Uv = FaceUVs[i],
                Color = faceColor
            };
            mesh.Vertices.Add(vertex);
            
            mesh.Indices.Add(vCount + 0); mesh.Indices.Add(vCount + 1); mesh.Indices.Add(vCount + 2);
            mesh.Indices.Add(vCount + 2); mesh.Indices.Add(vCount + 3); mesh.Indices.Add(vCount + 0);
        }
    }

    private bool FaceCulling(Vector3Int position)
    {
        throw new NotImplementedException();
    }

    private static readonly Vector2[] FaceUVs =
    [
        new Vector2(0, 0), // Нижній лівий
        new Vector2(1, 0), // Нижній правий
        new Vector2(1, 1), // Верхній правий
        new Vector2(0, 1)  // Верхній лівий
    ];
    
    private static readonly Vector3[] Normals =
    [
        Vector3.UnitY, 
        -Vector3.UnitY, 
        Vector3.UnitZ, 
        -Vector3.UnitZ,
        Vector3.UnitX, 
        -Vector3.UnitX 
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