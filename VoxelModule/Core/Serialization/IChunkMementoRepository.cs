using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace VoxelModule.Core.Serialization;

public interface IChunkMementoRepository
{
    ChunkMemento? Load(Vector3Int position);
    
    void Save(ChunkMemento memento);

    bool Exists(Vector3Int position);

}