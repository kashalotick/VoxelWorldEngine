using VoxelModule.Core;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine.Commands;

public enum ModifyMode
{
    ReplaceAll,
    ReplaceAir,
}

public record ModifyRegionCommand(
    IWorldRegion Region,
    Vector3Int InsertPosition,
    Vector3Int AreaSize,
    Voxel[] Data,
    ModifyMode Mode
)
    : ICommand
{
    public virtual void Execute()
    {
        
        switch (Mode)
        {
            case ModifyMode.ReplaceAll:
                ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAnyIfNotVoid);
                break;

            case ModifyMode.ReplaceAir:
                ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAirIfNotVoid);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), "Unknown ModifyMode");
        }
    }

    private void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        Voxel[] data,
        Func<Voxel, Voxel, bool> canReplace
    )
    {
        for (int z = 0; z < areaSize.Z; z++)
        {
            for (int y = 0; y < areaSize.Y; y++)
            {
                for (int x = 0; x < areaSize.X; x++)
                {
                    int index = x + y * areaSize.X + z * areaSize.X * areaSize.Y;
                    if (index >= data.Length) continue;

                    Voxel newVoxel = data[index];
                    Vector3Int pos = new Vector3Int(
                        insertPosition.X + x,
                        insertPosition.Y + y,
                        insertPosition.Z + z
                    );

                    Region.SetBlock(pos, newVoxel.BlockId, canReplace);
                }
            }
        }
    }

    private static bool CanPlaceInAnyIfNotVoid (Voxel oldVoxel, Voxel newVoxel) => !newVoxel.IsVoid;
    private static bool CanPlaceInAirIfNotVoid(Voxel oldVoxel, Voxel newVoxel) => oldVoxel.IsAir && !newVoxel.IsVoid;
}