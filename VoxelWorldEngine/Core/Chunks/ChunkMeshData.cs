using System.Numerics;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;

namespace VoxelWorldEngine.DataStructures.Special.Collections.Meshes;

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