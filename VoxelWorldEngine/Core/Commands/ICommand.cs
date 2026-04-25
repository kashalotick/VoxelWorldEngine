using VoxelWorldEngine.DataStructures.Common.Collections.Trees;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public interface ICommand
{
    void Execute();
}

public record BreakBlockCommand : PlaceBlockCommand
{
    public BreakBlockCommand(VoxelWorld world, Vector3Int voxelPositionIndex) : base(world,
        voxelPositionIndex, BlockId.Air)
    {
    }
}


public record PlaceBlockCommand : ICommand
{
    public VoxelWorld World;
    public Vector3Int VoxelPositionIndex;
    public BlockId BlockId;

    public PlaceBlockCommand(VoxelWorld world, Vector3Int voxelPositionIndex, BlockId blockId)
    {
        World = world;
        VoxelPositionIndex = voxelPositionIndex;
        BlockId = blockId;
    }

    public virtual void Execute()
    {
        World.PlaceBlock(VoxelPositionIndex, BlockId);
    }
}



// internal ??
public record ChunkLocalPlaceBlockCommand : ICommand
{
    public Chunk Chunk;
    public Vector3Int LocalVoxelPositionIndex;
    public BlockId BlockId;

    public ChunkLocalPlaceBlockCommand(Chunk chunk, Vector3Int localVoxelPositionIndex, BlockId blockId)
    {
        Chunk = chunk;
        LocalVoxelPositionIndex = localVoxelPositionIndex;
        BlockId = blockId;
    }

    public virtual void Execute()
    {
        // Chunk.PlaceBlock(VoxelPositionIndex, BlockId);
    }
}