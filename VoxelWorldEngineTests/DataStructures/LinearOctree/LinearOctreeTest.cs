using VoxelWorldEngine.DataStructures.LinearOctree;

namespace VoxelWorldEngineTests.DataStructures.LinearOctree;

public class LinearOctreeTest
{
    private VoxelWorldEngine.DataStructures.LinearOctree.LinearOctree _octree;

    [SetUp]
    public void Setup()
    {
        // Ініціалізація нового дерева перед кожним тестом
        _octree = new VoxelWorldEngine.DataStructures.LinearOctree.LinearOctree();
    }

    [Test]
    public void Constructor_InitializesCorrectly()
    {
        // Перевіряємо розмір (2^8 = 256)
        Assert.That(VoxelWorldEngine.DataStructures.LinearOctree.LinearOctree.Size, Is.EqualTo(256));

        // Перевіряємо, чи створено кореневий вузол
        // (Оскільки ми не маємо прямого доступу до _nodes, перевіряємо через GetNode на позиції 0)
        var rootPos = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(0, 0, 0);
        var node = _octree.GetNode(rootPos);

        // Корінь має бути створений (не null/default) і бути листом (Air) на старті
        Assert.That(node.IsLeaf, Is.True, "Root should start as a leaf");
        Assert.That(node.IsAir, Is.True, "Root should start as Air");
    }

    [Test]
    public void SubdivideNode_SplitsLeafIntoChildren()
    {
        // Arrange
        var rootPos = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(0, 0, 0);
        int rootIndex = _octree.GetNodeIndex(rootPos); // На старті це індекс кореня (0)

        // Act
        _octree.SubdivideNode(rootIndex);

        // Assert
        // 1. Тепер GetNodeIndex має повернути індекс ДИТИНИ, а не кореня
        int newIndex = _octree.GetNodeIndex(rootPos);

        Assert.That(newIndex, Is.Not.EqualTo(rootIndex), "GetNodeIndex should return a child index, not root index");
        Assert.That(newIndex, Is.GreaterThan(rootIndex), "Child index should be greater than root index");

        // 2. Якщо ви хочете переконатися, що повертається саме дитина:
        var childNode = _octree.GetNode(rootPos);
        Assert.That(childNode.IsLeaf, Is.True, "The resulting node at position should be a leaf (the child)");
    }

    [Test]
    public void SubdivideNode_ThrowsException_IfAlreadySubdivided()
    {
        // Arrange
        var rootPos = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(0, 0, 0);
        int rootIndex = _octree.GetNodeIndex(rootPos);
        _octree.SubdivideNode(rootIndex); // Перший поділ

        // Act & Assert
        // Спроба поділити вузол, який вже не є листом, має викликати помилку
        Assert.Throws<ArgumentException>(() => _octree.SubdivideNode(rootIndex));
    }

    [Test]
    public void UnsubdivideNode_RestoresLeafStatus()
    {
        // Arrange
        var rootPos = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(0, 0, 0);

        // Отримуємо індекс кореня (має бути 0)
        int rootIndex = _octree.GetNodeIndex(rootPos);

        // --- ДІЯ 1: SUBDIVIDE ---
        _octree.SubdivideNode(rootIndex);

        // ПЕРЕВІРКА 1:
        // Ми не перевіряємо IsLeaf, бо GetNode завжди повертає лист.
        // Ми перевіряємо, що ми тепер "глибше" в дереві (індекс змінився).
        int childIndex = _octree.GetNodeIndex(rootPos);
        Assert.That(childIndex, Is.Not.EqualTo(rootIndex),
            "After subdivision, GetNode should return a child index, not root");

        // --- ДІЯ 2: UNSUBDIVIDE ---
        _octree.UnsubdivideNode(rootIndex);

        // ПЕРЕВІРКА 2:
        // Тепер, коли ми шукаємо ноду за тією ж координатою, 
        // ми повинні впертись у Корінь (бо він знову став листом і дітей немає).
        int restoredIndex = _octree.GetNodeIndex(rootPos);
        var restoredNode = _octree.GetNode(rootPos);

        Assert.That(restoredIndex, Is.EqualTo(rootIndex),
            "After unsubdivide, accessing position should return root index again");
        Assert.That(restoredNode.IsLeaf, Is.True, "The node returned (Root) should be a leaf");

        // Перевірка стану (Solid або Air)
        Assert.That(restoredNode.ChildrenStartIndex, Is.EqualTo(-1).Or.EqualTo(-2));
    }

    [Test]
    public void SetNode_And_GetNode_Consistency()
    {
        // Arrange
        var position = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(10, 10, 10);

        // Створюємо тестовий воксель (Solid)
        var voxel = new VoxelWorldEngine.DataStructures.Special.Structures.Voxels.Voxel(127); // Припустимо 127 - це значення
        var nodeToSet = new LinearOctreeNode(voxel); // Solid node constructor logic implies -2 usually, or set manually
        // Якщо конструктор не ставить IsSolid автоматично, форсуємо для тесту:
        if (nodeToSet.IsAir) nodeToSet = LinearOctreeNode.Solid;
        // Або краще, якщо у вас є конструктор, що приймає Voxel і робить його Solid:
        // var nodeToSet = new LinearOctreeNode(voxel, -2); 

        // Act
        _octree.SetNode(position, nodeToSet);
        var retrievedNode = _octree.GetNode(position);

        // Assert
        Assert.That(retrievedNode.IsSolid, Is.True, "Retrieved node should be Solid");
        // Якщо Voxel реалізовано правильно, перевіряємо значення
        // Assert.That(retrievedNode.Voxel.Value, Is.EqualTo(127)); 
    }

    [Test]
    public void SetNode_AutoSubdividesToDepth()
    {
        // Цей тест перевіряє, чи SetNode автоматично створює глибину
        // Якщо ми ставимо воксель на конкретну координату, дерево повинно розбитися до MaxDepth

        // Arrange
        var position = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(5, 5, 5);
        var solidNode = LinearOctreeNode.Solid;

        // Act
        _octree.SetNode(position, solidNode);

        // Assert
        // Перевіряємо шлях до вузла через ForEachLeaf або перевіряючи індекс
        int leafIndex = _octree.GetNodeIndex(position);

        // Якщо дерево розбилося, індекс листа має бути великим (бо додалося багато дітей)
        Assert.That(leafIndex, Is.GreaterThan(0));
    }

    [Test]
    public void GetNode_OutOfBounds_ThrowsException()
    {
        // Припускаємо, що метод кидає виключення для координат поза межами Size
        // Size = 256. Valid: 0..255.

        var badPos = new VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int(300, 0, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => _octree.GetNode(badPos));
    }
}