using NUnit.Framework.Legacy;
using VoxelWorldEngine.DataStructures.LinearOctree;

namespace VoxelWorldEngineTests.DataStructures.LinearOctree;

public class LinearOctreeMathTest
{
    // ---------------------------------------------------------------------
    // Тести для GetOctant (Логіка 0b_ZYX)
    // Bit 2 (4) -> Z
    // Bit 1 (2) -> Y
    // Bit 0 (1) -> X
    // ---------------------------------------------------------------------

    [Test]
    [Description("Перевіряє розрахунок індексу октанту для порядку ZYX.")]
    // Size 16 -> Half 8
    [TestCase(2, 2, 2, 16, ExpectedResult = 0)] // 000 (всі < 8)
    [TestCase(10, 2, 2, 16, ExpectedResult = 1)] // 001 (X>=8) -> X=1
    [TestCase(2, 10, 2, 16, ExpectedResult = 2)] // 010 (Y>=8) -> Y=2
    [TestCase(2, 2, 10, 16, ExpectedResult = 4)] // 100 (Z>=8) -> Z=4
    [TestCase(10, 10, 10, 16, ExpectedResult = 7)] // 111 (Всі >= 8) -> 4+2+1 = 7
    [TestCase(10, 10, 2, 16, ExpectedResult = 3)] // 011 (X, Y set, Z low) -> 2+1 = 3
    [TestCase(2, 10, 10, 16, ExpectedResult = 6)] // 110 (Z, Y set, X low) -> 4+2 = 6
    [TestCase(10, 2, 10, 16, ExpectedResult = 5)] // 101 (Z, X set, Y low) -> 4+1 = 5
    public int GetOctant_CalculatesCorrectly_ZYX(int x, int y, int z, int size)
    {
        var point = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(x, y, z);
        return LinearOctreeMath.GetOctant(point, size);
    }

    [Test]
    [Description("Перевіряє граничні значення (середина = верхній октант).")]
    public void GetOctant_BoundaryCheck()
    {
        int size = 16; // Half = 8
        var point = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(8, 8, 8);

        int result = LinearOctreeMath.GetOctant(point, size);

        // 111 -> 7
        Assert.That(result, Is.EqualTo(7));
    }

    // ---------------------------------------------------------------------
    // Тести для GetOctantCorner (Логіка 0b_ZYX)
    // ---------------------------------------------------------------------

    [Test]
    [Description("Перевіряє позицію кута для порядку ZYX.")]
    // Size 32 -> Half 16
    // params: octant, size, expX, expY, expZ
    [TestCase(0, 32, 0, 0, 0)] // 000
    [TestCase(1, 32, 16, 0, 0)] // 001 -> X is high
    [TestCase(2, 32, 0, 16, 0)] // 010 -> Y is high
    [TestCase(4, 32, 0, 0, 16)] // 100 -> Z is high
    [TestCase(3, 32, 16, 16, 0)] // 011 -> Y, X high
    [TestCase(5, 32, 16, 0, 16)] // 101 -> Z, X high
    [TestCase(7, 32, 16, 16, 16)] // 111 -> All high
    public void GetOctantCorner_CalculatesCorrectly(int octantIndex, int size, int exX, int exY, int exZ)
    {
        var result = LinearOctreeMath.GetOctantCorner(octantIndex, size);

        
        
        Assert.That(result.X, Is.EqualTo(exX));
        Assert.That(result.Y, Is.EqualTo(exY));
        Assert.That(result.Z, Is.EqualTo(exZ));
    }

    // ---------------------------------------------------------------------
    // Тести для FindWayTo (Логіка 0b_ZYX)
    // ---------------------------------------------------------------------

    [Test]
    public void FindWayTo_StraightLine_Size4()
    {
        // Point (3, 3, 3) у просторі 4
        // Step 1 (Size 4, Half 2): (3,3,3) >= 2 -> 111 (ZYX) -> Index 7
        // Step 2 (Size 2, Half 1): (1,1,1) >= 1 -> 111 (ZYX) -> Index 7

        int size = 4;
        var point = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(3, 3, 3);
        int[] expected = new int[] { 7, 7 };

        int[] result = LinearOctreeMath.FindWayTo(point, size);

        Assert.That(result, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void FindWayTo_MixedPath_ZYX()
    {
        // Size 8 (Levels: 8->4, 4->2, 2->1). Depth = 3.
        // Point (1, 6, 2)

        // Level 0 (Size 8, Half 4):
        // X=1 (<4), Y=6 (>=4), Z=2 (<4)
        // Z=0, Y=1, X=0 -> 010 -> Index 2
        // New local point: (1, 6-4, 2) = (1, 2, 2)

        // Level 1 (Size 4, Half 2):
        // X=1 (<2), Y=2 (>=2), Z=2 (>=2)
        // Z=1, Y=1, X=0 -> 110 -> Index 6
        // New local point: (1, 2-2, 2-2) = (1, 0, 0)

        // Level 2 (Size 2, Half 1):
        // X=1 (>=1), Y=0 (<1), Z=0 (<1)
        // Z=0, Y=0, X=1 -> 001 -> Index 1

        // Result: [2, 6, 1]

        int size = 8;
        var point = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(1, 6, 2);
        int[] expected = new int[] { 2, 6, 1 };

        int[] result = LinearOctreeMath.FindWayTo(point, size);

        Assert.That(result, Is.EqualTo(expected).AsCollection, "Шлях розраховано неправильно для ZYX");
    }
}