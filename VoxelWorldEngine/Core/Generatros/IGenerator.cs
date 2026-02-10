namespace VoxelWorldEngine.Core.Generatros;

public interface IGenerator<T, TVector>
    where T : struct
    where TVector : struct
{
    T Approximate(TVector min, TVector max);
    bool IsUniform(TVector min, TVector max);
    T Maximum(TVector min, TVector max);
    T Minimum(TVector min, TVector max);

}