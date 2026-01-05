using VoxelWorldEngine.Core.Voxels;
using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Chunks;

public class Chunk : AbstractChunk, IDisposable
{
    public Voxel[,,] Voxels;



    public Chunk(Vector3Int position) : base(position)
    {
        Voxels = new Voxel[ChunkSize, ChunkSize, ChunkSize];
    }



    public Voxel GetVoxel(Vector3Int position)
    {
        return Voxels[position.X, position.Y, position.Z];
    }
    
    public bool MarkAsDirty()
    {
        if (IsDirty)
        {
            return false;
        }
        IsDirty = true;
        return true;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}