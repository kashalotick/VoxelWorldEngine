namespace VoxelWorldEngine.Utils;

public class TempDensityFunc
{
    float GetDensity(Vector3Int.Vector3Int pos)
    {
        return MathF.Cos(pos.X) + MathF.Cos(pos.Y) - pos.Z;
    }
}