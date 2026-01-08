using Vector3Int = VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int;

namespace VoxelWorldEngine.Core.Generator.Interfaces;

public interface IScalarFieldGenerator<T>
{
    T GetValue(Vector3Int position);
    T GetMinimum(Vector3Int a, Vector3Int b);
    T GetMaximum(Vector3Int a, Vector3Int b);
}