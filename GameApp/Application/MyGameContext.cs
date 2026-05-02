using LearningOpenTK.Core;
using LearningOpenTK.Engine;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.Resources.Textures.Array;
using LearningOpenTK.Engine.Resources.Textures.Atlas;

namespace GameApp.Application;

public class MyGameContext : GameContext
{
    public required AtlasTextureRepository UiAtlas { get; init; }
    public required AtlasTextureRepository BlockAtlas { get; init; }
    public required TextureArrayRepository TextureArrayRepository { get; init; }


    public static MyGameContext Create(int width, int height)
    {
        var textureRepository = new GLTextureRepository();
        var textureArrayRepository = new TextureArrayRepository();
        return new MyGameContext
        {
            ScreenWidth = width,
            ScreenHeight = height,
            TextureRepository = textureRepository,
            ShaderRepository = new ShaderRepository(),
            FontRepository = new FontRepository(),
            UiAtlas = new AtlasTextureRepository(textureRepository, "Ui"),
            BlockAtlas = new AtlasTextureRepository(textureRepository, "Blocks"),
            TextureArrayRepository = textureArrayRepository
        };
    }
}