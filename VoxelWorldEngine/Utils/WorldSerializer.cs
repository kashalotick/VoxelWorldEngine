using System.Text.Json;
using System.Text.Json.Serialization;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Voxels;

namespace VoxelWorldEngine.Utils;

// TODO: test serializer
public class WorldSerializer
{
    public void Save(World world, Vector3Int.Vector3Int position)
    {
        
        var chunks = SerializeChunks(world.Chunks);
        var serializableReady = SerializeWorld(position, chunks);
        
        SaveToDownloads(serializableReady);
    }

    private void SaveToDownloads(World_ serializableReady)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true 
        };
        
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string downloadsPath = Path.Combine(userProfile, "Downloads");
        string fileName = $"scalar_density_field_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
        string fullPath = Path.Combine(downloadsPath, fileName);

        var jsonString = JsonSerializer.Serialize(serializableReady, options);
        File.WriteAllText(fullPath, jsonString);

    }



    
    private World_ SerializeWorld(Vector3Int.Vector3Int pos, Dictionary<string, float[]> chunks)
    {
        int[] position = [pos.X, pos.Y, pos.Z];
        var world = new World_(position, chunks);
        return world;
    }
    private Dictionary<string, float[]> SerializeChunks(Dictionary<Vector3Int.Vector3Int, Chunk> chunks)
    {
        var dictionary = new Dictionary<string, float[]>();
        foreach (var pair in chunks)
        {
            var position = pair.Key;
            var chunk = pair.Value;

            var key = SerializeVector3Int(position);
            var array = Serialize3DVoxelArray(chunk.Voxels);
            dictionary[key] = array;
        }
        return dictionary;
    }

    private float[] Serialize3DVoxelArray(Voxel[,,] array)
    {
        var chunkSize = AbstractChunk.ChunkSize;
        var arraySize = chunkSize * chunkSize * chunkSize;
        var flat = new float[arraySize];
        
        for (var x = 0; x < chunkSize; x++)
        for (var y = 0; y < chunkSize; y++)
        for (var z = 0; z < chunkSize; z++)
        {
            var index = x + chunkSize * ( y + z * chunkSize);
            var voxel = array[x, y, z];
            var serialized = SerializeVoxel(voxel);
            flat[index] = serialized;
            
        }
        return flat;
    }
    private string SerializeVector3Int(Vector3Int.Vector3Int vector)
    {
        return $"{vector.X},{vector.Y},{vector.Z}";
    }
    private float SerializeVoxel(Voxel voxel)
    {
        return voxel.Density;
    }
}


public class World_
{
    [JsonPropertyName("observerPosition")]
    public int[] ObserverPosition { get; set; }

    [JsonPropertyName("chunks")]
    public Dictionary<string, float[]> Chunks { get; set; }

    public World_(int[] observerPosition, Dictionary<string, float[]> chunks)
    {
        ObserverPosition = observerPosition;
        Chunks = chunks;
    }
}

