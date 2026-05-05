using System.Collections.Concurrent;
using VoxelModule.Core.Serialization;
using VoxelModule.Core.Trees.LinearImplementation;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace GameApp.Content.Services;

public class ChunkMementoRepository : IChunkMementoRepository
{
    private readonly ConcurrentDictionary<Vector3Int, object> _chunkLocks = new();
    private readonly string _directory;

    public ChunkMementoRepository(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(directory);
    }

    public void Save(ChunkMemento memento)
    {
        Console.WriteLine($"Saving chunk {memento.Position}");
        Console.WriteLine(memento.Nodes.Length);
        var lockObj = _chunkLocks.GetOrAdd(memento.Position, _ => new object());
        lock (lockObj)
        {
            var path = GetPath(memento.Position);
            var tmp = path + ".tmp";

            using (var writer = new BinaryWriter(File.Open(tmp, FileMode.Create)))
            {
                writer.Write(memento.Nodes.Length);
                foreach (var node in memento.Nodes)
                {
                    writer.Write((byte)node.Data.BlockId);
                    writer.Write(node.ChildrenStartIndex);
                }
            }

            File.Move(tmp, path, true); // atomic
        }
    }

    public ChunkMemento? Load(Vector3Int position)
    {
        var path = GetPath(position);
        if (!File.Exists(path)) return null;

        using var reader = new BinaryReader(File.OpenRead(path));
        var count = reader.ReadInt32();
        var nodes = new LinearOctreeNode<Voxel>[count];
        for (var i = 0; i < count; i++)
        {
            var blockId = (BlockId)reader.ReadByte();
            var childrenStart = reader.ReadInt32();
            nodes[i] = new LinearOctreeNode<Voxel>(new Voxel(blockId), childrenStart);
        }

        return new ChunkMemento(position, nodes);
    }

    public bool Exists(Vector3Int position)
    {
        return File.Exists(GetPath(position));
    }

    private string GetPath(Vector3Int pos)
    {
        return Path.Combine(_directory, $"chunk_{pos.X}_{pos.Y}_{pos.Z}.bin");
    }
}