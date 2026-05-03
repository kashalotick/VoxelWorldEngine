using GameEngine.Core;
using GameEngine.Core.Resources;
using GameEngine.Engine;
using GameEngine.Engine.Resources.Fonts;
using GameEngine.Engine.Resources.Shaders;
using GameEngine.Engine.Resources.Textures;
using GameEngine.Engine.Resources.Textures.Array;
using GameEngine.Engine.Resources.Textures.Atlas;

namespace GameApp.Application;

public class MyGameContext : GameContext
{
    public required AtlasTextureRepository UiAtlas { get; init; }
    public required AtlasTextureRepository BlockAtlas { get; init; }
    public required TextureArrayRepository TextureArrayRepository { get; init; }


    public static MyGameContext Create(int width, int height)
    {
        var textureRepository = new ResourceRepository<GlTexture>(new GlTextureCreator());
        var shaderRepository = new ResourceRepository<Shader>(new ShaderCreator());
        var fontRepository = new ResourceRepository<Font>(new FontCreator());

        var textureArrayRepository = new TextureArrayRepository();
        return new MyGameContext
        {
            ScreenWidth = width,
            ScreenHeight = height,
            TextureRepository = textureRepository,
            ShaderRepository = shaderRepository,
            FontRepository = fontRepository,
            UiAtlas = new AtlasTextureRepository(textureRepository, "Ui"),
            BlockAtlas = new AtlasTextureRepository(textureRepository, "Blocks"),
            TextureArrayRepository = textureArrayRepository
        };
    }
}