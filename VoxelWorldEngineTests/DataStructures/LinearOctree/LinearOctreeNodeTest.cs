using VoxelWorldEngine.DataStructures.LinearOctree;

namespace VoxelWorldEngineTests.DataStructures.LinearOctree;

public class LinearOctreeNodeTest
{
    [Test]
    public void Static_Air_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var node = LinearOctreeNode.Air;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(node.ChildrenStartIndex, Is.EqualTo(-1), "Air ChildrenStartIndex should be -1");
            Assert.That(node.IsAir, Is.True, "IsAir should be true");
            Assert.That(node.IsLeaf, Is.True, "Air should be a Leaf");
            Assert.That(node.IsSolid, Is.False, "Air should not be Solid");
            Assert.That(node.Voxel.Density, Is.EqualTo(sbyte.MinValue), "Voxel value should be MinValue");
        });
    }

    [Test]
    public void Static_Solid_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var node = LinearOctreeNode.Solid;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(node.ChildrenStartIndex, Is.EqualTo(-2), "Solid ChildrenStartIndex should be -2");
            Assert.That(node.IsSolid, Is.True, "IsSolid should be true");
            Assert.That(node.IsLeaf, Is.True, "Solid should be a Leaf");
            Assert.That(node.IsAir, Is.False, "Solid should not be Air");
            Assert.That(node.Voxel.Density, Is.EqualTo(sbyte.MaxValue), "Voxel value should be MaxValue");
        });
    }

    [Test]
    public void Constructor_VoxelOnly_SetsDefaultIndex()
    {
        // Arrange
        var voxel = new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(10);

        // Act
        var node = new LinearOctreeNode(voxel);

        // Assert
        Assert.That(node.Voxel.Density, Is.EqualTo(10));
        Assert.That(node.ChildrenStartIndex, Is.EqualTo(-1),
            "Default index should be -1 (Air logic implied by constructor)");
        Assert.That(node.IsAir, Is.True);
    }

    [Test]
    public void Constructor_VoxelAndIndex_SetsProperties()
    {
        // Arrange
        var voxel = new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(5);
        int expectedIndex = 100;

        // Act
        var node = new LinearOctreeNode(voxel, expectedIndex);

        // Assert
        Assert.That(node.Voxel.Density, Is.EqualTo(5));
        Assert.That(node.ChildrenStartIndex, Is.EqualTo(expectedIndex));
        Assert.That(node.IsLeaf, Is.False, "Positive index implies it is a branch, not a leaf");
    }

    [TestCase(-1, true)] // Air
    [TestCase(-2, true)] // Solid
    [TestCase(-5, true)] // Invalid negative (still leaf by definition < 0)
    [TestCase(0, false)] // Root/Branch start
    [TestCase(100, false)] // Branch
    public void IsLeaf_ReturnsCorrectValue_BasedOnIndex(int index, bool expectedIsLeaf)
    {
        // Arrange
        var node = new LinearOctreeNode(new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(0), index);

        // Act & Assert
        Assert.That(node.IsLeaf, Is.EqualTo(expectedIsLeaf));
    }

    [TestCase(-1, true)]
    [TestCase(-2, false)]
    [TestCase(0, false)]
    public void IsAir_Check(int index, bool expected)
    {
        var node = new LinearOctreeNode(new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(0), index);
        Assert.That(node.IsAir, Is.EqualTo(expected));
    }

    [TestCase(-2, true)]
    [TestCase(-1, false)]
    [TestCase(10, false)]
    public void IsSolid_Check(int index, bool expected)
    {
        var node = new LinearOctreeNode(new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(0), index);
        Assert.That(node.IsSolid, Is.EqualTo(expected));
    }

    [Test]
    public void SetChildrenStartIndex_UpdatesProperty()
    {
        // Arrange
        var node = LinearOctreeNode.Air; // Start as -1

        // Act
        node.SetChildrenStartIndex(500);

        // Assert
        Assert.That(node.ChildrenStartIndex, Is.EqualTo(500));
        Assert.That(node.IsLeaf, Is.False);
    }

    [TestCase(100, 0, 100)] // Start 100, child 0 -> 100
    [TestCase(100, 7, 107)] // Start 100, child 7 -> 107
    public void GetChildIndex_ReturnsCorrectOffset(int startIndex, int octant, int expectedResult)
    {
        // Arrange
        var node = new LinearOctreeNode(new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(0), startIndex);

        // Act
        int result = node.GetChildIndex(octant);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}