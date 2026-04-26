namespace GameApp.Content;

public class WorldMeta
{
    public string Name { get; init; }
    public int Seed { get; init; }
    public double PlayTime { get; init; }
    public DateTime LastPlayed { get; init; }
    public int Slot { get; init; }
    
    public WorldMeta() { }

    public WorldMeta(int slot, string name, int seed, double playTime = 0, DateTime? lastPlayed = null)
    {
        Slot = slot;
        Name = name;
        Seed = seed;
        PlayTime = playTime;
        LastPlayed = lastPlayed ?? DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"WorldMeta(Slot={Slot}, Name='{Name}', Seed={Seed}, PlayTime={PlayTime}, LastPlayed={LastPlayed})";
    }
}