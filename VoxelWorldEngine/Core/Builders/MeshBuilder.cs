using System.Buffers;
using System.Numerics;
using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Builders;

public sealed class MeshGenerator
{
    private const int InitialCapacity = 4096;

    private static readonly Vector2[] FaceUVs =
    [
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1)
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
        [new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0)],
        [new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)],
        [new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)],
        [new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)],
        [new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1)],
        [new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0)]
    ];

    private static int GetFaceIndex(Vector3Int normal)
    {
        if (normal.Y > 0) return 0;
        if (normal.Y < 0) return 1;
        if (normal.Z > 0) return 2;
        if (normal.Z < 0) return 3;
        if (normal.X > 0) return 4;
        return 5;
    }

    public RentedMeshData Generate(IVoxelOctree octree)
    {
        var builder = new Builder();
        try
        {
            octree.Accept(builder);
            return builder.ToMeshData();
        }
        catch
        {
            builder.Dispose();
            throw;
        }
    }

    private sealed class Builder : IOctreeVisitor<Voxel>, IDisposable
    {
        private ChunkVertex[] _vertices;
        private uint[] _indices;
        private int _vertexCount;
        private int _indexCount;
        private bool _ownershipTransferred;

        public Builder()
        {
            _vertices = ArrayPool<ChunkVertex>.Shared.Rent(InitialCapacity);
            _indices = ArrayPool<uint>.Shared.Rent(InitialCapacity);
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
            var neighbor = node.GetNeighbor(normal);
            if (neighbor == null) return false;

            var neighborFace = GetNeighborFaceIndices(node, normal);
            var queryResult = neighbor.Query(neighborFace.min, neighborFace.max);
            foreach (var v in queryResult)
            {
                if (node.Data.IsTransparent)
                {
                    if (!v.IsTransparent) return false;
                }
                else
                {
                    if (v.IsTransparent || v.IsAir) return false;
                }
            }

            return true;
        }

        private (Vector3Int min, Vector3Int max) GetNeighborFaceIndices(IOctreeNodeReadonly<Voxel> node, Vector3Int normal)
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

            if (normal.X != 0)
            {
                neighborMin.X = neighborMax.X = (normal.X > 0) ? node.MaxIndex.X + 1 : node.MinIndex.X - 1;
            }
            else if (normal.Y != 0)
            {
                neighborMin.Y = neighborMax.Y = (normal.Y > 0) ? node.MaxIndex.Y + 1 : node.MinIndex.Y - 1;
            }
            else if (normal.Z != 0)
            {
                neighborMin.Z = neighborMax.Z = (normal.Z > 0) ? node.MaxIndex.Z + 1 : node.MinIndex.Z - 1;
            }

            return (neighborMin, neighborMax);
        }

        private void AddFace(IOctreeNodeReadonly<Voxel> node, Vector3Int normal)
        {
            EnsureCapacity(4, 6);

            var facePosition = (Vector3)node.MinIndex;
            var faceNormal = (Vector3)normal;
            var vCount = (uint)_vertexCount;
            var normalIndex = GetFaceIndex(normal);

            for (int i = 0; i < 4; i++)
            {
                var offset = FaceVertices[normalIndex][i];
                _vertices[_vertexCount++] = new ChunkVertex
                {
                    Position = facePosition + node.Size * offset,
                    Normal = faceNormal,
                    Uv = FaceUVs[i] * node.Size,
                    BlockId = node.Data.BlockId
                };
            }

            _indices[_indexCount++] = vCount + 0;
            _indices[_indexCount++] = vCount + 1;
            _indices[_indexCount++] = vCount + 2;
            _indices[_indexCount++] = vCount + 2;
            _indices[_indexCount++] = vCount + 3;
            _indices[_indexCount++] = vCount + 0;
        }

        private void EnsureCapacity(int vertexAdditional, int indexAdditional)
        {
            var requiredVertices = _vertexCount + vertexAdditional;
            if (requiredVertices > _vertices.Length)
            {
                var newVertices = ArrayPool<ChunkVertex>.Shared.Rent(requiredVertices * 2);
                Array.Copy(_vertices, newVertices, _vertexCount);
                ArrayPool<ChunkVertex>.Shared.Return(_vertices, clearArray: false);
                _vertices = newVertices;
            }

            var requiredIndices = _indexCount + indexAdditional;
            if (requiredIndices > _indices.Length)
            {
                var newIndices = ArrayPool<uint>.Shared.Rent(requiredIndices * 2);
                Array.Copy(_indices, newIndices, _indexCount);
                ArrayPool<uint>.Shared.Return(_indices, clearArray: false);
                _indices = newIndices;
            }
        }

        public RentedMeshData ToMeshData()
        {
            _ownershipTransferred = true;
            return new RentedMeshData(_vertices, _vertexCount, _indices, _indexCount);
        }

        public void Dispose()
        {
            if (_ownershipTransferred)
            {
                return;
            }

            ArrayPool<ChunkVertex>.Shared.Return(_vertices, clearArray: false);
            ArrayPool<uint>.Shared.Return(_indices, clearArray: false);
        }
    }
}
