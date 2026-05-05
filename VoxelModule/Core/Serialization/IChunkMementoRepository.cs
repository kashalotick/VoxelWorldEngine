using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Core.Serialization;

public interface IChunkMementoRepository
{
    ChunkMemento? Load(Vector3Int position);
    
    void Save(ChunkMemento memento);

    bool Exists(Vector3Int position);

}