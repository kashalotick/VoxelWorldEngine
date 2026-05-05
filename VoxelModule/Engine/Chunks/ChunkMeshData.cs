namespace VoxelModule.Engine.Chunks;

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