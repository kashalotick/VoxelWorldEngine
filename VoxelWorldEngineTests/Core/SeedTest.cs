using VoxelWorldEngine.Core;

namespace VoxelWorldEngineTests.Core;

// TODO: make tests
public class SeedTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Seed_Constructor_Seed()
    {
        var seedValue = 123456789123456789;
        var seed = new Seed(seedValue);
        Assert.That(seed.Value, Is.EqualTo(seedValue));
    }

    [Test]
    public void Seed_Constructor_Seed_Min_Exception()
    {
        var seedValue = 123456789123;
        Assert.Throws<ArgumentOutOfRangeException>(() => new Seed(seedValue));
    }

    [Test]
    public void Seed_Constructor_Seed_Max_Exception()
    {
        var seedValue = 1234567891234567890;
        Assert.Throws<ArgumentOutOfRangeException>(() => new Seed(seedValue));
    }
}