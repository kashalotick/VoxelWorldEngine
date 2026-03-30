namespace VoxelWorldEngine.DataStructures.Special;

public class World
{
    public string Name { get; private set; }
    public int Seed { get; private set; }
    public double PlayTime { get; private set; }
    
    
    public World(string name, int seed)
    {
        Name = name;
        Seed = seed;
    }
    
    
    
    
}