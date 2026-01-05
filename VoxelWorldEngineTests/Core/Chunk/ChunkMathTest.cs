using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Utils;
using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngineTests.Core.Chunk;

// TODO: make tests
public class ChunkMathTest
{
    [SetUp]
    public void Setup()
    {
    }

    [TestFixture]
    public class GetChunkPositionTests
    {
        [Test]
        [TestCase(0, 0, 0, ExpectedResult = "0, 0, 0")]      // Точка нуль
        [TestCase(15, 15, 15, ExpectedResult = "0, 0, 0")]   // Край першого чанка
        [TestCase(16, 0, 0, ExpectedResult = "1, 0, 0")]     // Початок другого чанка
        [TestCase(-1, -1, -1, ExpectedResult = "-1, -1, -1")] // Від'ємна зона (важливо!)
        [TestCase(-16, 0, 0, ExpectedResult = "-1, 0, 0")]    // Межа від'ємного чанка
        [TestCase(-17, 0, 0, ExpectedResult = "-2, 0, 0")]    // Перехід у наступний від'ємний
        public string Should_ReturnCorrectChunkCoordinate(int x, int y, int z)
        {
            var voxelPos = new Vector3Int(x, y, z);

            var result = ChunkMath.GetChunkPosition(voxelPos);

            return $"{result.X}, {result.Y}, {result.Z}";
        }
        
        // [Test]
        // [TestCase(0, 0, 0, ExpectedResult = "0, 0, 0")]      // Точка нуль
        // [TestCase(15, 15, 15, ExpectedResult = "0, 0, 0")]   // Край першого чанка
        // [TestCase(16, 0, 0, ExpectedResult = "1, 0, 0")]     // Початок другого чанка
        // [TestCase(-1, -1, -1, ExpectedResult = "-1, -1, -1")] // Від'ємна зона (важливо!)
        // [TestCase(-16, 0, 0, ExpectedResult = "-1, 0, 0")]    // Межа від'ємного чанка
        // [TestCase(-17, 0, 0, ExpectedResult = "-2, 0, 0")]    // Перехід у наступний від'ємний
        // public string Should_ReturnCorrectChunkCoordinate_Overload(int x, int y, int z)
        // {
        //     var result = ChunkMath.GetChunkPosition(x, y, z);
        //
        //     return $"{result.X}, {result.Y}, {result.Z}";
        // }
    }
}