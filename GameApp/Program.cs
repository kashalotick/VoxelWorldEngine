using GameApp.Content.Scenes;
using LearningOpenTK.Core;


namespace GameApp;

public class Program
{
    public static void Main(string[] args)
    {
        var defaultScene = (GameContext gc) => new MainMenu(gc);

        var game = new Game(1200, 900, "Voxel engine test",
            defaultScene);

        game.Run();
    }
}