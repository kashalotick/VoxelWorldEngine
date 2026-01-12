namespace VoxelWorldEngineTests.DataStructures.Vector2Int;

public class Vector2IntTest
{
    // --- 1. Constructors & Static Properties ---

    [Test]
    public void Constructor_SingleValue_SetsXandY()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5);
        Assert.That(v.X, Is.EqualTo(5));
        Assert.That(v.Y, Is.EqualTo(5));
    }

    [Test]
    public void Constructor_XY_SetsComponents()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 3);
        Assert.That(v.X, Is.EqualTo(2));
        Assert.That(v.Y, Is.EqualTo(3));
    }

    [Test]
    public void StaticProperties_HaveCorrectValues()
    {
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.Zero, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(0, 0)));
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.One, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 1)));
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.UnitX, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 0)));
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.UnitY, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(0, 1)));
    }

    // --- 2. Equality & Overrides ---

    [Test]
    public void Equals_And_Operators_WorkCorrectly()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 2);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 2);
        var v3 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4);

        // Equals(Vector2Int)
        Assert.That(v1.Equals(v2), Is.True);
        Assert.That(v1.Equals(v3), Is.False);

        // Equals(object)
        Assert.That(v1.Equals((object)v2), Is.True);
        Assert.That(v1.Equals(new object()), Is.False);

        // Operators == and !=
        Assert.That(v1 == v2, Is.True);
        Assert.That(v1 != v3, Is.True);
    }

    [Test]
    public void GetHashCode_SameValues_SameHash()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 20);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 20);

        Assert.That(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()));
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, -10);
        Assert.That(v.ToString(), Is.EqualTo("5, -10"));
    }

    // --- 3. Arithmetic Operators ---

    [Test]
    public void Operator_Addition()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 2);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4);
        Assert.That(v1 + v2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(4, 6)));
    }

    [Test]
    public void Operator_Subtraction()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 5);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 1);
        Assert.That(v1 - v2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4)));
    }

    [Test]
    public void Operator_UnaryNegation()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, -5);
        Assert.That(-v, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(-1, 5)));
    }

    [Test]
    public void Operator_Multiplication_Vector()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 3);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(4, 5);
        Assert.That(v1 * v2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(8, 15)));
    }

    [Test]
    public void Operator_Multiplication_Scalar()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 3);

        // v * int
        Assert.That(v * 2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(4, 6)));

        // int * v
        Assert.That(3 * v, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(6, 9)));
    }

    [Test]
    public void Operator_Division_Vector()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 20);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 5);
        Assert.That(v1 / v2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 4)));
    }

    [Test]
    public void Operator_Division_Scalar()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 20);
        Assert.That(v / 2, Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 10)));
    }

    [Test]
    public void Operator_DivisionByZero_ThrowsException()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 10);
        Assert.Throws<DivideByZeroException>(() =>
        {
            var _ = v / 0;
        });
    }

    // --- 4. Math Methods ---

    [Test]
    public void Dot_CalculatesCorrectly()
    {
        var v1 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(2, 3);
        var v2 = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(4, 5);
        // 2*4 + 3*5 = 8 + 15 = 23
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.Dot(v1, v2), Is.EqualTo(23));
    }

    [Test]
    public void LengthSquared_CalculatesCorrectly()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4); // 3-4-5 triangle
        // 3*3 + 4*4 = 9 + 16 = 25
        Assert.That(v.LengthSquared(), Is.EqualTo(25));
    }

    [Test]
    public void Length_CalculatesCorrectly()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4);
        Assert.That(v.Length(), Is.EqualTo(5f).Within(0.0001f));
    }

    [Test]
    public void DistanceSquared_CalculatesCorrectly()
    {
        var a = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(0, 0);
        var b = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(3, 4);
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.DistanceSquared(a, b), Is.EqualTo(25));
    }

    [Test]
    public void Distance_CalculatesCorrectly()
    {
        var a = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 1);
        var b = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 11); // dist = 10
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.Distance(a, b), Is.EqualTo(10f).Within(0.0001f));
    }

    [Test]
    public void Abs_ReturnsPositiveComponents()
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(-5, 10);
        Assert.That(VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int.Abs(v), Is.EqualTo(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 10)));
    }

    // --- 5. Comparison Operators (<, >) ---
    // Note: Comparison is based on Magnitude (LengthSquared)

    [Test]
    public void Operators_Comparison_BasedOnLength()
    {
        var vSmall = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(1, 1); // LenSq = 2
        var vBig = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 5); // LenSq = 50

        Assert.That(vBig > vSmall, Is.True);
        Assert.That(vSmall < vBig, Is.True);

        Assert.That(vSmall > vBig, Is.False);
    }

    // --- 6. Bounds & Logic ---

    [TestCase(0, 0, 10, true)] // Start
    [TestCase(9, 9, 10, true)] // End inclusive
    [TestCase(10, 5, 10, false)] // X out
    [TestCase(5, 10, 10, false)] // Y out
    [TestCase(-1, 0, 10, false)] // Negative X
    [TestCase(0, -1, 10, false)] // Negative Y
    public void IsInBounds_WorksCorrectly(int x, int y, int size, bool expected)
    {
        var v = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(x, y);
        Assert.That(v.IsInBounds(size), Is.EqualTo(expected));
        // Примітка: (uint) cast обробляє від'ємні числа як дуже великі, тому -1 стає > size
    }

    [Test]
    public void IsBetween_WorksCorrectly()
    {
        var min = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(0, 0);
        var max = new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 10);

        // Inside
        Assert.That(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 5).IsBetween(min, max), Is.True);

        // Edge cases (Inclusive Start)
        Assert.That(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(0, 0).IsBetween(min, max), Is.True);

        // Edge cases (Exclusive End)
        Assert.That(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(10, 10).IsBetween(min, max), Is.False);

        // Outside
        Assert.That(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(-1, 5).IsBetween(min, max), Is.False);
        Assert.That(new VoxelWorldEngine.DataStructures.Vector2Int.Vector2Int(5, 11).IsBetween(min, max), Is.False);
    }
}