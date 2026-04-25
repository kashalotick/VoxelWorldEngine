using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core.Serialization;

public interface IChunkMementoRepository
{
    ChunkMemento? Load(Vector3Int position);
    
    void Save(ChunkMemento memento);

    bool Exists(Vector3Int position);

}