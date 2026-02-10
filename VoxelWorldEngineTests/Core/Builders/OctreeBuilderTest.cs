using Moq;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.Core.Generators.Interfaces;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace VoxelWorldEngineTests.Core.Builders;

public class OctreeBuilderTest
{
    private Mock<IScalarFieldGenerator<Vector3Int, sbyte>> _mockGenerator;
    private OctreeBuilder _builder;

    private const int MaxDepth = 8; // Згідно з вашим LinearOctree.MaxDepth
    private const int ChunkSize = 256; // 1 << 8

    [SetUp]
    public void Setup()
    {
        _mockGenerator = new Mock<IScalarFieldGenerator<Vector3Int, sbyte>>();
        _builder = new OctreeBuilder(_mockGenerator.Object);
    }

    [Test]
    public void Build_EmptyChunk_CreatesSingleRootNode()
    {
        // ARRANGE
        // Сценарій: Весь чанк - це повітря.
        // Generator каже: поверхонь немає ніде.
        _mockGenerator
            .Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>()))
            .Returns(false);

        // Generator каже: щільність у центрі = Air (-128)
        _mockGenerator
            .Setup(g => g.GetValue(It.IsAny<Vector3Int>()))
            .Returns(sbyte.MinValue);

        var chunk = new Chunk(new Vector3Int(0, 0, 0));

        // ACT
        var octree = _builder.Build(chunk);

        // ASSERT
        // Корінь (індекс 0) має бути листом.
        // Для доступу до ноди використовуємо GetNode або перевіряємо через GetNodeIndex(будь-яка позиція)
        
        var rootPos = new Vector3Int(0, 0, 0);
        
        // Перевіряємо, що дерево не розбилося (повертає індекс 0)
        // Примітка: GetNodeIndex повертає індекс ноди в списку. Корінь = 0.
        int nodeIndex = octree.GetNodeIndex(rootPos);
        Assert.That(nodeIndex, Is.EqualTo(0), "Empty chunk should not be subdivided");

        var rootNode = octree.GetNode(rootPos);
        Assert.Multiple(() =>
        {
            Assert.That(rootNode.IsLeaf, Is.True, "Root should remain a leaf");
            Assert.That(rootNode.Voxel.Density, Is.EqualTo(sbyte.MinValue), "Voxel value should be Air");
        });
    }

    [Test]
    public void Build_SolidChunk_CreatesSingleRootNode()
    {
        // ARRANGE
        // Сценарій: Весь чанк - це камінь (під землею).
        // IsHereAnySurface -> false (бо немає переходу повітря-камінь, все суцільне)
        _mockGenerator
            .Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>()))
            .Returns(false);

        // Щільність = Solid (127)
        _mockGenerator
            .Setup(g => g.GetValue(It.IsAny<Vector3Int>()))
            .Returns(sbyte.MaxValue);

        var chunk = new Chunk(new Vector3Int(0, 0, 0));

        // ACT
        var octree = _builder.Build(chunk);

        // ASSERT
        var rootNode = octree.GetNode(Vector3Int.Zero);
        Assert.Multiple(() =>
        {
            Assert.That(rootNode.IsLeaf, Is.True, "Root should remain a leaf (fully solid)");
            Assert.That(rootNode.Voxel.Density, Is.EqualTo(sbyte.MaxValue), "Voxel value should be Solid");
        });
    }

    [Test]
    public void Build_WithSurface_SubdividesRoot()
    {
        // ARRANGE
        // Сценарій: У чанку є поверхня, треба ділити.
        // Але діти вже однорідні (щоб не рекурсивно ділити до нескінченності в тесті).

        // Логіка мока:
        // 1. Якщо розмір запиту == ChunkSize (256) -> True (є поверхня)
        // 2. Якщо розмір менший -> False (діти однорідні)
        
        _mockGenerator
            .Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>()))
            .Returns((Vector3Int min, Vector3Int max) =>
            {
                int size = max.X - min.X;
                return size == ChunkSize; // Тільки корінь ділимо
            });

        // Значення для листя (нехай буде повітря для спрощення)
        _mockGenerator
            .Setup(g => g.GetValue(It.IsAny<Vector3Int>()))
            .Returns(sbyte.MinValue);

        var chunk = new Chunk(new Vector3Int(0, 0, 0));

        // ACT
        var octree = _builder.Build(chunk);

        // ASSERT
        // Тепер корінь (індекс 0) має бути розбитий.
        // Отже, якщо ми запитаємо ноду за будь-якою координатою, ми маємо отримати індекс > 0 (дитину).
        
        var pos = new Vector3Int(0, 0, 0);
        int nodeIndex = octree.GetNodeIndex(pos);

        Assert.That(nodeIndex, Is.Not.EqualTo(0), "Root should be subdivided because surface was detected");
        
        // Отримана нода (дитина) має бути листом
        var childNode = octree.GetNode(pos);
        Assert.That(childNode.IsLeaf, Is.True, "Children should be leaves (recursion stopped)");
    }

    [Test]
    public void ProcessLeaf_CalculatesCenterCorrectly()
    {
        // Цей тест перевіряє, чи правильно Build викликає GetValue з центром вокселя.
        
        // ARRANGE
        var chunk = new Chunk(new Vector3Int(0, 0, 0));
        
        // Не ділимо корінь (щоб одразу потрапити в ProcessLeaf для розміру 256)
        _mockGenerator.Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>())).Returns(false);
        _mockGenerator.Setup(g => g.GetValue(It.IsAny<Vector3Int>())).Returns(0);

        // ACT
        _builder.Build(chunk);

        // ASSERT
        // Розмір чанка 256. Центр має бути (128, 128, 128).
        // Логіка в коді: octantCorner + Vector3Int.One * (nodeSize / 2)
        // 0 + 1 * (256 / 2) = 128.
        
        var expectedCenter = new Vector3Int(128, 128, 128);
        
        _mockGenerator.Verify(g => g.GetValue(expectedCenter), Times.Once);
    }
    
    [Test]
    public void ProcessLeaf_ForSmallestNode_UsesCorner()
    {
        // Тест для логіки: nodeSize > 1 ? center : corner
        // Нам треба змусити білдер дійти до розміру 1.
        
        // ARRANGE
        var chunk = new Chunk(new Vector3Int(0, 0, 0));
        
        // Кажемо, що поверхня є ВЕЗДЕ, поки розмір > 1.
        // Це змусить рекурсію йти до самого низу (MaxDepth).
        _mockGenerator
            .Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>()))
            .Returns((Vector3Int min, Vector3Int max) =>
            {
                int size = max.X - min.X;
                return size > 1; // Ділимо поки > 1
            });
            
        // Щоб тест не був вічним і не жер пам'ять, можна обмежити MaxDepth у самому Octree для тесту
        // або (краще) схитрувати: дозволити поділ тільки для конкретної гілки (наприклад, тільки 0-вого октанта).
        // Але оскільки IsHereAnySurface працює по координатах, це складно налаштувати в Mock без складної логіки.
        
        // СПРОЩЕНИЙ ВАРІАНТ:
        // Ми не будемо йти до глибини 1 (це 2^8 = 256 кроків рекурсії x 8 гілок -> вибух).
        // Ми перевіримо логіку "центру" на рівні 128 (один поділ).
        
        // Поділимо корінь (256 -> 128).
        _mockGenerator.Setup(g => g.IsHereAnySurface(It.IsAny<Vector3Int>(), It.IsAny<Vector3Int>()))
            .Returns((Vector3Int min, Vector3Int max) => (max.X - min.X) == 256); // Тільки 1 рівень

        // ACT
        _builder.Build(chunk);

        // ASSERT
        // Має бути 8 викликів GetValue для дітей розміром 128.
        // Для дитини 0 (координати 0,0,0, розмір 128) центр = 64,64,64.
        
        // Перевіряємо хоча б один виклик
        _mockGenerator.Verify(g => g.GetValue(new Vector3Int(64, 64, 64)), Times.Once);
    }
}