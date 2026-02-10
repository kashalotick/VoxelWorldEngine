using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngineTests.DataStructures.Common.Structures.Vectors;

[TestFixture]
public class Vector3IntTest
{
    [Test]
    public void Constructor_WithSingleValue_SetsAllFields()
    {
        var vector = new Vector3Int(10);

        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(10));
            Assert.That(vector.Y, Is.EqualTo(10));
            Assert.That(vector.Z, Is.EqualTo(10));
        });
    }

    [Test]
    public void Constructor_WithThreeValues_SetsFieldsCorrectly()
    {
        var vector = new Vector3Int(1, 2, 3);

        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(2));
            Assert.That(vector.Z, Is.EqualTo(3));
        });
    }

    [Test]
    public void StaticProperties_ZeroAndOne_ReturnCorrectValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Vector3Int.Zero, Is.EqualTo(new Vector3Int(0, 0, 0)));
            Assert.That(Vector3Int.One, Is.EqualTo(new Vector3Int(1, 1, 1)));
        });
    }

    [Test]
    public void Constant_Count_IsThree()
    {
        Assert.That(Vector3Int.Count, Is.EqualTo(3));
    }

    [TestCase(3, 4, 0, ExpectedResult = 25)]
    [TestCase(1, 1, 1, ExpectedResult = 3)]
    [TestCase(0, 0, 0, ExpectedResult = 0)]
    public int LengthSquared_Calculates_CorrectValue(int x, int y, int z)
    {
        return new Vector3Int(x, y, z).LengthSquared();
    }

    [TestCase(3, 4, 0, ExpectedResult = 5.0f)]
    [TestCase(0, 0, 0, ExpectedResult = 0.0f)]
    public float Length_Calculates_CorrectValue(int x, int y, int z)
    {
        return new Vector3Int(x, y, z).Length();
    }

    [TestCase(1, 2, 3, 4, 5, 6, ExpectedResult = 27)] // (1-4)^2 + (2-5)^2 + (3-6)^2 = 9+9+9
    public int DistanceSquared_Calculates_CorrectValue(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        return Vector3Int.DistanceSquared(new Vector3Int(x1, y1, z1), new Vector3Int(x2, y2, z2));
    }

    [TestCase(1, 2, 2, 1, 1, 1, ExpectedResult = 5)] // 1*1 + 2*1 + 2*1 = 5
    [TestCase(2, 3, 4, 5, 6, 7, ExpectedResult = 56)] // 2*5 + 3*6 + 4*7 = 10 + 18 + 28 = 56
    [TestCase(1, 0, 0, 0, 1, 0, ExpectedResult = 0)] // Перпендикулярні вектори
    public int Dot_Calculates_CorrectValue(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        return Vector3Int.Dot(new Vector3Int(x1, y1, z1), new Vector3Int(x2, y2, z2));
    }

    [TestCase(-1, 2, -3, 1, 2, 3)]
    [TestCase(0, 0, 0, 0, 0, 0)]
    public void Abs_Returns_PositiveValues(int x, int y, int z, int ex, int ey, int ez)
    {
        var result = Vector3Int.Abs(new Vector3Int(x, y, z));
        Assert.That(result, Is.EqualTo(new Vector3Int(ex, ey, ez)));
    }

    [Test]
    public void Equals_SameValues_ReturnsTrue()
    {
        var v1 = new Vector3Int(1, 2, 3);
        var v2 = new Vector3Int(1, 2, 3);

        Assert.Multiple(() =>
        {
            Assert.That(v1 == v2, Is.True);
            Assert.That(v1.Equals(v2), Is.True);
            Assert.That(v1.Equals((object)v2), Is.True);
            Assert.That(v1 != v2, Is.False);
        });
    }

    [TestCase(1, 2, 3, 0, 2, 3)]
    [TestCase(1, 2, 3, 1, 0, 3)]
    [TestCase(1, 2, 3, 1, 2, 0)]
    public void Equals_DifferentValues_ReturnsFalse(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        var v1 = new Vector3Int(x1, y1, z1);
        var v2 = new Vector3Int(x2, y2, z2);

        Assert.Multiple(() =>
        {
            Assert.That(v1 == v2, Is.False);
            Assert.That(v1.Equals(v2), Is.False);
            Assert.That(v1 != v2, Is.True);
        });
    }

    [Test]
    public void Addition_Calculates_CorrectValue()
    {
        var v1 = new Vector3Int(1, 2, 3);
        var v2 = new Vector3Int(10, 20, 30);
        var expected = new Vector3Int(11, 22, 33);

        Assert.That(v1 + v2, Is.EqualTo(expected));
    }

    [Test]
    public void Subtraction_Calculates_CorrectValue()
    {
        var v1 = new Vector3Int(10, 20, 30);
        var v2 = new Vector3Int(1, 2, 3);
        var expected = new Vector3Int(9, 18, 27);

        Assert.That(v1 - v2, Is.EqualTo(expected));
    }

    [Test]
    public void UnaryNegation_Returns_InvertedVector()
    {
        var v = new Vector3Int(1, -2, 3);
        Assert.That(-v, Is.EqualTo(new Vector3Int(-1, 2, -3)));
    }

    [TestFixture]
    public class MultiplicationTests
    {
        [Test]
        public void VectorByVector_Calculates_CorrectValue()
        {
            var v1 = new Vector3Int(2, 3, 4);
            var v2 = new Vector3Int(5, 6, 7);
            var expected = new Vector3Int(10, 18, 28);

            Assert.That(v1 * v2, Is.EqualTo(expected));
        }

        [Test]
        public void VectorByInt_Calculates_CorrectValue()
        {
            var v = new Vector3Int(1, 2, 3);
            var multiplier = 10;
            var expected = new Vector3Int(10, 20, 30);

            Assert.That(v * multiplier, Is.EqualTo(expected));
        }

        [Test]
        public void IntByVector_Calculates_CorrectValue()
        {
            var v = new Vector3Int(1, 2, 3);
            var multiplier = 10;
            var expected = new Vector3Int(10, 20, 30);

            Assert.That(multiplier * v, Is.EqualTo(expected));
        }
    }

    [TestFixture]
    public class DivisionTests
    {
        [Test]
        public void VectorByVector_Calculates_CorrectValue()
        {
            var v1 = new Vector3Int(10, 20, 30);
            var v2 = new Vector3Int(2, 5, 3);
            var expected = new Vector3Int(5, 4, 10);

            Assert.That(v1 / v2, Is.EqualTo(expected));
        }

        [Test]
        public void VectorByInt_Calculates_CorrectValue()
        {
            var v1 = new Vector3Int(10, 20, 30);
            var divisor = 10;
            var expected = new Vector3Int(1, 2, 3);

            Assert.That(v1 / divisor, Is.EqualTo(expected));
        }
    }
}