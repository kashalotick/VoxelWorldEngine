using GameEngine.Engine.Resources.Textures.Array;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace GameApp.Application;

public class BlockRegistry
{
    public (TextureArray array, TextureArrayMap<BlockId> map) Build(TextureArrayRepository repository)
    {
        return new TextureArrayMap<BlockId>(repository, "Blocks")
            .Set(BlockId.Air, "Air")
            .Set(BlockId.Stone, "Stone")
            .Set(BlockId.Dirt, "Dirt")
            .Set(BlockId.Grass, "Grass")
            .Set(BlockId.Bricks, "Bricks")
            .Set(BlockId.Wood, "Wood")
            .Set(BlockId.Leaves, "Leaves")
            .Set(BlockId.Glass, "Glass")
            .Set(BlockId.Planks, "Planks")
            .Set(BlockId.Mixed, "Mixed")
            .Build();
    }
}