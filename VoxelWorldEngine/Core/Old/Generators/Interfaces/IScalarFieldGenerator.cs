namespace VoxelWorldEngine.Core.Generators.Interfaces;

// TODO: write doc
public interface IScalarFieldGenerator<in TVector, TValue>
{
    TValue GetValue(TVector position);
    // TValue[] GetArea(TVector a, TVector b);
    (TValue min, TValue max) GetMinMax(TVector a, TVector b);
    bool IsHereAnySurface(TVector a, TVector b);

}