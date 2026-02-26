using VoxelWorldEngine.Core;

namespace GameApp.Content.Services;


/// <summary>
///  generate, save, load worlds
/// </summary>
public class WorldService
{
    
    
    public World GenerateWorld()
    {
        var seed = new Random().Next();
        return GenerateWorld(seed);
    }
    
    public World GenerateWorld(int seed)
    {
        return new World(seed);
    }
}