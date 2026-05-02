using System.Numerics;
using VoxelModule.DataStructures.Special.Structures.Vertices;

namespace VoxelModule.DataStructures.Special.Collections.Meshes;

public class ChunkMeshData
{
    public uint[] Indices;      // capacity одразу
    public ChunkVertex[] Vertices;

    public ChunkMeshData(uint[] indices, ChunkVertex[] vertices)
    {
        Indices = indices;
        Vertices = vertices;
    }
}