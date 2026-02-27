using System.Numerics;
using GameApp.Old;
using LearningOpenTK.Core;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Meshes;
using LearningOpenTK.Resources;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameApp;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game(1200, 900, "Voxel engine test",
            (w, h, input) => new DemoScene(w, h, input));
        game.Run();
    }
}