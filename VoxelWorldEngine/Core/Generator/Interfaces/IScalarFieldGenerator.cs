using Vector3Int = VoxelWorldEngine.DataStructures.Vector3Int.Vector3Int;

namespace VoxelWorldEngine.Core.Generator.Interfaces;

public interface IScalarFieldGenerator<in TVector, TValue>
{
    TValue GetValue(TVector position);
    (TValue min, TValue max) GetMinMax(TVector a, TVector b);
}