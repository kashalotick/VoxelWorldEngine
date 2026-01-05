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


    // TODO: test for serialization
    public void Generate()
    {
        for (var x = 0; x < ChunkSize; x++)
        for (var y = 0; y < ChunkSize; y++)
        {
            var surfaceHeight = GetHeight(x, y);
            for (var z = 0; z < ChunkSize; z++)
            {
                var density = GetDensity(surfaceHeight, z);
                var material = density == 0f ? Material.Air : Material.Stone;
                var voxel = new Voxel(density, material);
                Voxels[x, y, z] = voxel;
            }
        }

    }

    public float GetHeight(int x, int y)
    {
        var surfaceHeight = (x + y) * 0.0625f;

        return surfaceHeight;
    }
    public float GetDensity(float surfaceHeight, int z)
    {
        if (z < surfaceHeight)
        {
            return 1;
        }
        if (Math.Abs(surfaceHeight - z) < 1)
        {
            return surfaceHeight - z;
        }
        return 0;
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