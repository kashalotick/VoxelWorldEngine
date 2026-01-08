namespace VoxelWorldEngine.Core;

public struct Seed
{
    private const long Min = 100_000_000_000_000_000L;
    private const long Max = 999_999_999_999_999_999L;

    public readonly long Value;

    public Seed()
    {
        Value = new Random().NextInt64(Min, Max);
    }

    public Seed(long seed)
    {
        if (seed < Min || seed > Max) throw new ArgumentOutOfRangeException(nameof(seed));

        Value = seed;
    }
}