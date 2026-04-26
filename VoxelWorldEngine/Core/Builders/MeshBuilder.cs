using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Builders;

public class MeshBuilder : IMeshBuilder
{
    private List<uint> _indices = new(4096);
    private List<ChunkVertex> _vertices = new(4096);
    
    public ChunkMeshData Build(IVoxelOctree octree)
    {
        octree.Accept(this);

        var mesh = new ChunkMeshData(_indices.ToArray(), _vertices.ToArray());
        _indices.Clear();
        _vertices.Clear();
        return mesh;
    }
    

    public void Visit(IOctreeNodeReadonly<Voxel> node)
    {
        if (node.IsLeaf && !node.Data.IsAir)
        {
            AddFaces(node);
        }
    }
    
    private void AddFaces(IOctreeNodeReadonly<Voxel> node)
    {
        for (int i = 0; i < 6; i++)
        {
            var normal = Normals[i];

            if (FaceCulling(node, normal)) continue;

            AddFace(node, normal);
        }
    }
    
    private bool FaceCulling(IOctreeNodeReadonly<Voxel> node, Vector3Int normal)
    {
        var neighbor =  node.GetNeighbor(normal);
        if (neighbor == null) return false;
        // return !neighbor.Data.IsAir;
    
        var neighborFace = GetNeighborFaceIndices(node, normal);
        var queryResult = neighbor.Query(neighborFace.min, neighborFace.max);
        return queryResult.All(v => !v.IsAir);
    }
    
    
    public (Vector3Int min, Vector3Int max) GetNeighborFaceIndices(IOctreeNodeReadonly<Voxel> node, Vector3Int normal)
    {
        Vector3Int neighborMin;
        Vector3Int neighborMax;
    
        Vector3Int startFaceMin = new Vector3Int(
            normal.X > 0 ? node.MaxIndex.X : node.MinIndex.X,
            normal.Y > 0 ? node.MaxIndex.Y : node.MinIndex.Y,
            normal.Z > 0 ? node.MaxIndex.Z : node.MinIndex.Z
        );

        Vector3Int startFaceMax = new Vector3Int(
            normal.X < 0 ? node.MinIndex.X : node.MaxIndex.X,
            normal.Y < 0 ? node.MinIndex.Y : node.MaxIndex.Y,
            normal.Z < 0 ? node.MinIndex.Z : node.MaxIndex.Z
        );

        neighborMin = startFaceMin + normal;
        neighborMax = startFaceMax + normal;
        
        if (normal.X != 0) {
            neighborMin.X = neighborMax.X = (normal.X > 0) ? node.MaxIndex.X + 1 : node.MinIndex.X - 1;
        }
        else if (normal.Y != 0) {
            neighborMin.Y = neighborMax.Y = (normal.Y > 0) ? node.MaxIndex.Y + 1 : node.MinIndex.Y - 1;
        }
        else if (normal.Z != 0) {
            neighborMin.Z = neighborMax.Z = (normal.Z > 0) ? node.MaxIndex.Z + 1 : node.MinIndex.Z - 1;
        }

        return (neighborMin, neighborMax);
    }
    
    private void AddFace(IOctreeNodeReadonly<Voxel> node, Vector3Int normal)
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