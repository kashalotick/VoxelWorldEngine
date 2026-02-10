using DotnetNoise;
using VoxelWorldEngine.Core.Generators;
using Vector2Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector2Int;

namespace VoxelWorldEngineTests.Core.Generator;

public class HeighMapGeneratorTest
{
    // Цей тест перевіряє, чи правильно ми налаштовуємо об'єкт FastNoise
    [Test]
    public void Constructor_SetsNoiseParametersCorrectly()
    {
        // Arrange
        var noise = new FastNoise();

        // Act
        var generator = new HeightMapGenerator(noise);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noise.UsedNoiseType, Is.EqualTo(FastNoise.NoiseType.Simplex));
            Assert.That(noise.FractalTypeMethod, Is.EqualTo(FastNoise.FractalType.Fbm));
            Assert.That(noise.Octaves, Is.EqualTo(1));
            Assert.That(noise.Frequency, Is.EqualTo(0.01f));
        });
    }

    // Інтеграційний тест: перевіряємо, що бібліотека працює і вертає дані
    [Test]
    public void GetValue_ReturnsValidFloat()
    {
        // Arrange
        var noise = new FastNoise();
        var generator = new HeightMapGenerator(noise);
        var pos = new Vector2Int(10, 20);

        // Act
        float value = generator.GetValue(pos);

        // Assert
        // Шум зазвичай повертає значення в діапазоні [-1, 1] або схожому
        Assert.That(value, Is.InRange(-100f, 100f));
        Assert.That(value, Is.Not.NaN);
    }

    [Test]
    public void GetMinMax_ReturnsValidRange()
    {
        // Arrange
        var noise = new FastNoise();
        var generator = new HeightMapGenerator(noise);
        var a = new Vector2Int(0, 0);
        var b = new Vector2Int(10, 10);

        // Act
        var (min, max) = generator.GetMinMax(a, b);

        // Assert
        Assert.That(min, Is.LessThanOrEqualTo(max));
        Assert.That(min, Is.Not.NaN);
        Assert.That(max, Is.Not.NaN);
    }
}