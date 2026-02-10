namespace VoxelWorldEngineTests.DataStructures.Voxel;

public class VoxelTest
{
    [SetUp]
    public void Setup()
    {
    }
    
    [TestCase(1, ExpectedResult = 1)]
    [TestCase(5, ExpectedResult = 5)]
    [TestCase(0, ExpectedResult = 0)]
    [TestCase(-0, ExpectedResult = 0)]
    [TestCase(-1, ExpectedResult = -1)]
    [TestCase(-128, ExpectedResult = -128)]
    public sbyte Constructor_Sbyte(sbyte density)
    {
        var voxel = new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(density);
        return voxel.Density;
    }
}