using GameApp.Content.Scenes;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Scenes;
using LearningOpenTK.Engine.Resources.Fonts;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.Resources.Textures.Array;
using LearningOpenTK.Resources.Repositories;


namespace GameApp;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game<MyGameContext>(
            1200,
            900,
            "Voxel engine test",
            gc => new MainMenu(gc),
            (w, h) =>
            {
                var textureRepository = new GLTextureRepository();
                var textureArrayRepository = new TextureArrayRepository();
                return new MyGameContext
                {
                    ScreenWidth = w,
                    ScreenHeight = h,
                    TextureRepository = textureRepository,
                    ShaderRepository = new ShaderRepository(),
                    FontRepository = new FontRepository(),
                    UiAtlas = new AtlasTextureRepository(textureRepository, "Ui"),
                    TextureArrayRepository = textureArrayRepository,
                    // BlockArray = new BlockMapper().Build(textureArrayRepository);
                    // BlockAtlas = new AtlasTextureRepository(textureRepository, "Blocks"),
                };
            });

        game.Run();
    }
}