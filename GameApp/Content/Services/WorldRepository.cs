using System.Text.Json;
using VoxelModule.Core;
using VoxelModule.Core.Serialization;
using VoxelModule.Engine;

namespace GameApp.Content.Services;

/// <summary>
///     generate, save, load worlds
/// </summary>
public class WorldRepository
{
    private const int SlotCount = 3;
    private readonly string _savesPath;

    public WorldRepository(string savesPath = "saves")
    {
        _savesPath = savesPath;
    }

    public WorldMeta?[] GetSlots()
    {
        var slots = new WorldMeta?[SlotCount];
        for (var i = 0; i < SlotCount; i++)
            slots[i] = ReadMeta(i);
        return slots;
    }

    public WorldMeta CreateSlot(int slot, string name, int seed)
    {
        ValidateSlot(slot);

        var info = new WorldMeta(slot, name, seed);

        Directory.CreateDirectory(SlotPath(slot));
        WriteMeta(info);

        return info;
    }

    public void DeleteSlot(int slot)
    {
        ValidateSlot(slot);
        var path = SlotPath(slot);
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    public bool SlotExists(int slot)
    {
        ValidateSlot(slot);
        return File.Exists(MetaPath(slot));
    }

    // --- world
    public WorldMeta UpdateMeta(WorldMeta meta, double playTime)
    {
        var newPlayTime = meta.PlayTime + playTime;
        var newMeta = new WorldMeta(meta.Slot, meta.Name, meta.Seed, newPlayTime, DateTime.UtcNow);

        WriteMeta(newMeta);
        return newMeta;
    }

    public VoxelWorld LoadWorld(WorldMeta meta)
    {
        var voxelWorld = new VoxelWorld(meta.Seed);
        return voxelWorld;
    }

    public WorldState? LoadState(int slot)
    {
        var path = StatePath(slot);
        if (!File.Exists(path)) return null;
        var dto = JsonSerializer.Deserialize<WorldStateDto>(File.ReadAllText(path), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        });
        return dto == null ? null : (WorldState)dto;
    }

    public void SaveState(int slot, WorldState state)
    {
        var json = JsonSerializer.Serialize((WorldStateDto)state, new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        });
        File.WriteAllText(StatePath(slot), json);
    }

    // --- internal ---


    private WorldMeta? ReadMeta(int slot)
    {
        var path = MetaPath(slot);
        if (!File.Exists(path)) return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<WorldMeta>(json);
    }

    private void WriteMeta(WorldMeta meta)
    {
        var json = JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(MetaPath(meta.Slot), json);
    }

    private string SlotPath(int slot)
    {
        return Path.Combine(_savesPath, $"slot_{slot}");
    }

    private string ChunksPath(int slot)
    {
        return Path.Combine(SlotPath(slot), "chunks");
    }

    private string MetaPath(int slot)
    {
        return Path.Combine(SlotPath(slot), "meta.json");
    }

    private string StatePath(int slot)
    {
        return Path.Combine(SlotPath(slot), "state.json");
    }

    private static void ValidateSlot(int slot)
    {
        if (slot < 0 || slot >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(slot), $"Slot must be 0–{SlotCount - 1}");
    }

    public IChunkMementoRepository GetChunkRepository(int slot)
    {
        return new ChunkMementoRepository(ChunksPath(slot));
    }
}