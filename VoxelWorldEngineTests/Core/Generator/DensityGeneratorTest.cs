using Moq;
using VoxelWorldEngine.Core.Generators;
using VoxelWorldEngine.Core.Generators.Interfaces;
using VoxelWorldEngine.DataStructures.Vector2Int;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngineTests.Core.Generator;

public class DensityGeneratorTest
{
    private Mock<IScalarFieldGenerator<Vector2Int, float>> _heightMapMock;
    private DensityGenerator _generator;

    private const sbyte Solid = sbyte.MaxValue;
    private const sbyte Air = sbyte.MinValue;

    [SetUp]
    public void Setup()
    {
        _heightMapMock = new Mock<IScalarFieldGenerator<Vector2Int, float>>();
        _generator = new DensityGenerator(_heightMapMock.Object);
    }

    [Test]
    public void GetValue_BelowHeight_ReturnsSolid()
    {
        // Arrange
        // Карта висот каже, що тут висота 10.0
        _heightMapMock.Setup(h => h.GetValue(It.IsAny<Vector2Int>())).Returns(10.0f);
        
        var position = new Vector3Int(5, 5, 5); // Z = 5 (нижче 10)

        // Act
        var result = _generator.GetValue(position);

        // Assert
        Assert.That(result, Is.EqualTo(Solid));
    }

    [Test]
    public void GetValue_AboveHeight_ReturnsAir()
    {
        // Arrange
        _heightMapMock.Setup(h => h.GetValue(It.IsAny<Vector2Int>())).Returns(10.0f);
        
        var position = new Vector3Int(5, 5, 15); // Z = 15 (вище 10)

        // Act
        var result = _generator.GetValue(position);

        // Assert
        Assert.That(result, Is.EqualTo(Air));
    }


   
    // --- Тести для GetMinMax ---

    [Test]
    public void GetMinMax_FullyUnderGround_ReturnsSolidSolid()
    {
        // Arrange
        // Область висот ландшафту: від 20 до 30
        _heightMapMock.Setup(h => h.GetMinMax(It.IsAny<Vector2Int>(), It.IsAny<Vector2Int>()))
            .Returns((20f, 30f));

        // Ми запитуємо область Z: від 0 до 10 (це глибоко під землею)
        var start = new Vector3Int(0, 0, 0);
        var end = new Vector3Int(10, 10, 10);

        // Act
        var (min, max) = _generator.GetMinMax(start, end);

        // Assert
        // Очікуємо (Solid, Solid) -> if (z.max < height.min)
        Assert.That(min, Is.EqualTo(Solid));
        Assert.That(max, Is.EqualTo(Solid));
    }

    [Test]
    public void GetMinMax_FullyInSky_ReturnsAirAir()
    {
        // Arrange
        // Область висот ландшафту: від 20 до 30
        _heightMapMock.Setup(h => h.GetMinMax(It.IsAny<Vector2Int>(), It.IsAny<Vector2Int>()))
            .Returns((20f, 30f));

        // Ми запитуємо область Z: від 40 до 50 (це високо в небі)
        var start = new Vector3Int(0, 0, 40);
        var end = new Vector3Int(10, 10, 50);

        // Act
        var (min, max) = _generator.GetMinMax(start, end);

        // Assert
        // Очікуємо (Air, Air) -> if (z.min > height.max)
        Assert.That(min, Is.EqualTo(Air));
        Assert.That(max, Is.EqualTo(Air));
    }

    [Test]
    public void GetMinMax_CrossingSurface_ReturnsSolidAir()
    {
        // Arrange
        // Область висот ландшафту: від 20 до 30
        _heightMapMock.Setup(h => h.GetMinMax(It.IsAny<Vector2Int>(), It.IsAny<Vector2Int>()))
            .Returns((20f, 30f));

        // Ми запитуємо область Z: від 10 до 40 (перетинає землю)
        var start = new Vector3Int(0, 0, 10);
        var end = new Vector3Int(10, 10, 40);

        // Act
        var (min, max) = _generator.GetMinMax(start, end);

        // Assert
        Assert.That(min, Is.EqualTo(Air));
        Assert.That(max, Is.EqualTo(Solid));
    }

    // --- Тести для IsHereAnySurface ---

    [Test]
    public void IsHereAnySurface_ReturnsTrue_WhenSolidAndAirMixed()
    {
        // Arrange
        // Імітуємо перетин поверхні (Solid, Air)
        // Для цього налаштуємо MinMax так, щоб висота була 25, а Z запит від 10 до 40
        _heightMapMock.Setup(h => h.GetMinMax(It.IsAny<Vector2Int>(), It.IsAny<Vector2Int>()))
            .Returns((25f, 25f));
        
        var start = new Vector3Int(0, 0, 10);
        var end = new Vector3Int(10, 10, 40);

        // Act
        bool result = _generator.IsHereAnySurface(start, end);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsHereAnySurface_ReturnsFalse_WhenFullySolid()
    {
        // Arrange
        // Висота 50, запит Z до 10 (під землею)
        _heightMapMock.Setup(h => h.GetMinMax(It.IsAny<Vector2Int>(), It.IsAny<Vector2Int>()))
            .Returns((50f, 50f));
        
        var start = new Vector3Int(0, 0, 0);
        var end = new Vector3Int(10, 10, 10);

        // Act
        bool result = _generator.IsHereAnySurface(start, end);

        // Assert
        Assert.That(result, Is.False);
    }
}