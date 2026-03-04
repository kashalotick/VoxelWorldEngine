using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

// TODO: implement logic
public class Chunk
{
    public MeshData Mesh { get; set; }
    public VoxelOctree Octree { get; set; }
    public Vector3Int Position { get; set; }
    public Vector3Int GlobalPosition => ChunkToGlobal(Position);
    
    // public Vector3Int Min => GlobalPosition;
    // public Vector3Int Max => GlobalPosition + Vector3Int.One * ChunkSize;


    public Chunk(Vector3Int position)
    {
        Position = position;
    }


    
    public static int ChunkSize => Constants.ChunkSize;

    public static Vector3Int ChunkToGlobal(Vector3Int position)
    {
        return position * ChunkSize;
    }

    public static Vector3Int GlobalToChunk(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.FloorDiv(position.X, ChunkSize),
            MathHelper.FloorDiv(position.Y, ChunkSize),
            MathHelper.FloorDiv(position.Z, ChunkSize)
        );
    }


    public static Vector3Int GlobalToLocal(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.Mod(position.X, ChunkSize),
            MathHelper.Mod(position.Y, ChunkSize),
            MathHelper.Mod(position.Z, ChunkSize)
        );
    }
}