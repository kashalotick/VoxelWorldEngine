using GameApp.Application;
using GameApp.Content.Scenes;
using GameEngine.Core;
using GameEngine.Engine;
using GameEngine.Engine.Resources.Textures.Atlas;

namespace GameApp;

public class Program
{
    public static void Main(string[] args)
    {
        var atlasBuilder = new AtlasBuilder();
        atlasBuilder.Prebuild();
        
        var game = new Game<MyGameContext>(
            1200,
            900,
            "Voxel engine test",
            gc => new MainMenuScene(gc),
            MyGameContext.Create,
            "Assets/IconDark.png"
        );

        game.Run();
    }
}