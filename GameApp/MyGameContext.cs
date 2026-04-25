using LearningOpenTK.Core;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.Resources.Textures.Array;

namespace GameApp;

public class MyGameContext : GameContext
{
    public required AtlasTextureRepository UiAtlas { get; init; }
    public required AtlasTextureRepository BlockAtlas { get; init; }
    
    public required TextureArrayRepository TextureArrayRepository { get; init; }
    // public required TextureArray BlockArray { get; init; }
    
}