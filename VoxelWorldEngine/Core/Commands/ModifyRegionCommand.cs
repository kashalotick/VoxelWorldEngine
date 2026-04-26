using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

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
                Region.ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAnyIfNotVoid);
                break;

            case ModifyMode.ReplaceAir:
                Region.ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAirIfNotVoid);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), "Unknown ModifyMode");
        }
    }

    private static bool CanPlaceInAnyIfNotVoid (Voxel oldVoxel, Voxel newVoxel) => !newVoxel.IsVoid;
    private static bool CanPlaceInAirIfNotVoid(Voxel oldVoxel, Voxel newVoxel) => oldVoxel.IsAir && !newVoxel.IsVoid;
}