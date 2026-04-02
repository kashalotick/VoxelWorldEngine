using System.Text.Json;
using VoxelWorldEngine.Core;

namespace GameApp.Content.Services;


/// <summary>
///  generate, save, load worlds
/// </summary>
public class WorldRepository
{
    private const int SlotCount = 3;
    private readonly string _savesPath;

    public WorldRepository(string savesPath = "saves")
    {
        _savesPath = savesPath;
    }
    
    public WorldInfo?[] GetSlots()
    {
        var slots = new WorldInfo?[SlotCount];
        for (int i = 0; i < SlotCount; i++)
            slots[i] = ReadMeta(i);
        return slots;
    }
    
    public WorldInfo CreateSlot(int slot, string name, int seed)
    {
        ValidateSlot(slot);

        var info = new WorldInfo(slot, name, seed);

        Directory.CreateDirectory(SlotPath(slot));
        WriteMeta(info);

        return info;
    }

    public void DeleteSlot(int slot)
    {
        ValidateSlot(slot);
        var path = SlotPath(slot);
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
    }

    public bool SlotExists(int slot)
    {
        ValidateSlot(slot);
        return File.Exists(MetaPath(slot));
    }
    
    
    // --- internal ---

    
    private WorldInfo? ReadMeta(int slot)
    {
        var path = MetaPath(slot);
        if (!File.Exists(path)) return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<WorldInfo>(json);
    }

    private void WriteMeta(WorldInfo info)
    {
        var json = JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(MetaPath(info.Slot), json);
    }

    private string SlotPath(int slot) => Path.Combine(_savesPath, $"slot_{slot}");
    private string MetaPath(int slot) => Path.Combine(SlotPath(slot), "meta.json");

    private static void ValidateSlot(int slot)
    {
        if (slot < 0 || slot >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(slot), $"Slot must be 0–{SlotCount - 1}");
    }
    
    
    public VoxelWorld GenerateWorld(int? seed = null)
    {
        return new VoxelWorld(seed ??  new Random().Next());
    }
}