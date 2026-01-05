using VoxelWorldEngine.Utils.Vector3Int;

namespace VoxelWorldEngine.Core.Chunks;

public class ChunkLoader
{
    private Vector3Int _observerPosition;
    private int LoadRadius => Config.ChunkLoadRadius;

    public Queue<Chunk> ChunksOnLoad0;


    public ChunkLoader()
    {
        ChunksOnLoad0 = new Queue<Chunk>();

        _observerPosition = Vector3Int.Zero;
    }

    public void SetObserverPosition(Vector3Int observerPosition)
    {
        _observerPosition = observerPosition;
    }

    
    public List<Chunk> GetExtra(Dictionary<Vector3Int, Chunk> chunks)
    {
        var extraChunks = new List<Chunk>();
        foreach (var (position, chunk) in chunks)
        {
            if (!IsInLoadRadius(position))
            {
                extraChunks.Add(chunk);
            }
        }
        return extraChunks;
    }

    public List<Chunk> GetMissing(Dictionary<Vector3Int, Chunk> chunks)
    {
        var missingChunks = new List<Chunk>();
        
        var xStart = _observerPosition.X - LoadRadius;
        var xEnd = _observerPosition.X + LoadRadius;
        var yStart = _observerPosition.Y - LoadRadius;
        var yEnd = _observerPosition.Y + LoadRadius;
        var zStart = _observerPosition.Z - LoadRadius;
        var zEnd = _observerPosition.Z + LoadRadius;


        for (var x = xStart; x <= xEnd; x++)
        for (var y = yStart; y <= yEnd; y++)
        for (var z = zStart; z <= zEnd; z++)
        {
            var position = new Vector3Int(x, y, z);
            if (!chunks.ContainsKey(position))
            {
                var newChunk = new Chunk(position);
                missingChunks.Add(newChunk);
            }
        }
        
        return missingChunks;
    }



    // TODO: make loading optimization (under ground etc.)
    public bool IsVisible(Vector3Int chunkPosition)
    {
        var isInLoadRadius = IsInLoadRadius(chunkPosition);
        return isInLoadRadius;
    }

    public bool IsInLoadRadius(Vector3Int chunkPosition)
    {
        var distanceSquared = Vector3Int.DistanceSquared(chunkPosition, _observerPosition);
        return distanceSquared <= LoadRadius * LoadRadius;
    }
}

