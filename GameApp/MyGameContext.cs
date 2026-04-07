using LearningOpenTK.Core;
using LearningOpenTK.Engine.Resources.Textures;

namespace GameApp;

public class MyGameContext : GameContext
{
    public required AtlasTextureRepository UiAtlas { get; init; }
    public required AtlasTextureRepository BlockAtlas { get; init; }
    
}