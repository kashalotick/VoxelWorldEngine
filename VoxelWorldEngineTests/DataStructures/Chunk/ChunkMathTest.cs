using VoxelWorldEngine.DataStructures.Chunk;

namespace VoxelWorldEngineTests.DataStructures.Chunk;

public class ChunkMathTest
{
    // Розмір чанка для перевірки розрахунків (1 << 8 = 256)
    private const int ChunkSize = 256;

    // ---------------------------------------------------------------------
    // 1. ChunkToGlobal
    // Логіка: ChunkPos * 256
    // ---------------------------------------------------------------------

    [Test]
    [Description("Перевіряє перетворення координат чанка в глобальні координати.")]
    // 0 -> 0
    [TestCase(0, 0, 0, ExpectedResult = new int[] { 0, 0, 0 })]
    // 1 -> 256
    [TestCase(1, 2, 3, ExpectedResult = new int[] { 256, 512, 768 })]
    // -1 -> -256
    [TestCase(-1, -1, -1, ExpectedResult = new int[] { -256, -256, -256 })]
    // Mix
    [TestCase(-2, 0, 10, ExpectedResult = new int[] { -512, 0, 2560 })]
    public int[] ChunkToGlobal_CalculatesCorrectly(int x, int y, int z)
    {
        var chunkPos = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(x, y, z);
        var globalPos = ChunkMath.ChunkToGlobal(chunkPos);

        // Використовуємо X, Y, Z з великої літери
        return new int[] { globalPos.X, globalPos.Y, globalPos.Z };
    }

    // ---------------------------------------------------------------------
    // 2. GlobalToChunk
    // Логіка: FloorDiv(GlobalPos, 256)
    // ---------------------------------------------------------------------

    [Test]
    [Description("Перевіряє визначення індексу чанка за глобальною позицією.")]
    // Позитивні (всередині першого чанка [0..255])
    [TestCase(0, 0, 0, ExpectedResult = new int[] { 0, 0, 0 })]
    [TestCase(255, 255, 255, ExpectedResult = new int[] { 0, 0, 0 })]

    // Позитивні (перехід у наступний чанк [256..])
    [TestCase(256, 256, 256, ExpectedResult = new int[] { 1, 1, 1 })]
    [TestCase(512, 256, 0, ExpectedResult = new int[] { 2, 1, 0 })] // 512/256=2

    // Від'ємні (Найважливіше: -1 має бути в чанку -1, а не 0)
    [TestCase(-1, -1, -1, ExpectedResult = new int[] { -1, -1, -1 })]
    [TestCase(-256, -256, -256, ExpectedResult = new int[] { -1, -1, -1 })] // Границя зліва
    [TestCase(-257, -10, -500, ExpectedResult = new int[] { -2, -1, -2 })] // -257->ch-2, -500->ch-2
    public int[] GlobalToChunk_CalculatesCorrectly(int x, int y, int z)
    {
        var globalPos = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(x, y, z);
        var chunkPos = ChunkMath.GlobalToChunk(globalPos);

        return new int[] { chunkPos.X, chunkPos.Y, chunkPos.Z };
    }

    // ---------------------------------------------------------------------
    // 3. GlobalToLocal
    // Логіка: Mod(GlobalPos, 256). Результат завжди [0..255].
    // ---------------------------------------------------------------------

    [Test]
    [Description("Перевіряє отримання локальних координат всередині чанка.")]
    // Позитивні прості
    [TestCase(10, 20, 30, ExpectedResult = new int[] { 10, 20, 30 })]
    // Граничні позитивні
    [TestCase(256, 257, 258, ExpectedResult = new int[] { 0, 1, 2 })] // 256%256=0

    // Від'ємні (Циклічність)
    // -1 локально це 255 (останній блок у чанку ліворуч)
    [TestCase(-1, -1, -1, ExpectedResult = new int[] { 255, 255, 255 })]
    // -256 локально це 0 (перший блок чанка ліворуч)
    [TestCase(-256, -256, -256, ExpectedResult = new int[] { 0, 0, 0 })]
    // Мікс: -257 це як -1 відносно границі -256, тобто 255
    [TestCase(-257, 10, -1, ExpectedResult = new int[] { 255, 10, 255 })]
    public int[] GlobalToLocal_CalculatesCorrectly(int x, int y, int z)
    {
        var globalPos = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(x, y, z);
        var localPos = ChunkMath.GlobalToLocal(globalPos);

        // Перевіряємо, чи ми в межах [0..255]
        Assert.That(localPos.X, Is.GreaterThanOrEqualTo(0).And.LessThan(ChunkSize));
        Assert.That(localPos.Y, Is.GreaterThanOrEqualTo(0).And.LessThan(ChunkSize));
        Assert.That(localPos.Z, Is.GreaterThanOrEqualTo(0).And.LessThan(ChunkSize));

        return new int[] { localPos.X, localPos.Y, localPos.Z };
    }

    // ---------------------------------------------------------------------
    // 4. Інтеграційний тест (Consistency Check)
    // Global = (Chunk * 256) + Local
    // ---------------------------------------------------------------------

    [Test]
    [TestCase(1000, -500, 255)] // Випадкові координати
    [TestCase(-257, 0, -1)]
    public void Consistency_Check(int x, int y, int z)
    {
        var globalOriginal = new VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int(x, y, z);

        var chunkPos = ChunkMath.GlobalToChunk(globalOriginal);
        var localPos = ChunkMath.GlobalToLocal(globalOriginal);

        // Реконструкція: Chunk * Size + Local
        int recX = (chunkPos.X * ChunkSize) + localPos.X;
        int recY = (chunkPos.Y * ChunkSize) + localPos.Y;
        int recZ = (chunkPos.Z * ChunkSize) + localPos.Z;

        Assert.That(recX, Is.EqualTo(globalOriginal.X), "X reconstruction failed");
        Assert.That(recY, Is.EqualTo(globalOriginal.Y), "Y reconstruction failed");
        Assert.That(recZ, Is.EqualTo(globalOriginal.Z), "Z reconstruction failed");
    }
}