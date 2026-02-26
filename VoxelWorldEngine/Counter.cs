namespace VoxelWorldEngine;

public static class Counter
{
    private static readonly Dictionary<CounterType, int> _cache = [];

    public static void Increment(CounterType type)
    {
        _cache.TryAdd(type, 0);

        _cache[type]++;
    }

    public static void Display()
    {
        foreach (var i in _cache)
        {
            Console.WriteLine($"{i.Key:G}: {i.Value}");
        }
    }
}

public enum CounterType
{
    VoxelOctreeGetData,
    VoxelOctreeBuild,
}