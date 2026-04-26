using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Commands;

public enum ModifyMode
{
    ReplaceAll,
    ReplaceAir,
}

public record ModifyRegionCommand(
    VoxelWorld World,
    Vector3Int InsertPosition,
    Vector3Int AreaSize,
    Voxel[] Data,
    ModifyMode Mode
)
    : ICommand
{
    public virtual void Execute()
    {
        Console.WriteLine(
            $"[ModifyRegionCommand] Executing ModifyRegion at {InsertPosition}, mode: {Mode}, area size: {AreaSize}");
        switch (Mode)
        {
            case ModifyMode.ReplaceAll:
                World.ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAnyIfNotVoid);
                break;

            case ModifyMode.ReplaceAir:
                World.ModifyArea(InsertPosition, AreaSize, Data, CanPlaceInAirIfNotVoid);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), "Unknown ModifyMode");
        }
    }

    private static bool CanPlaceInAnyIfNotVoid (Voxel oldVoxel, Voxel newVoxel) => !newVoxel.IsVoid;
    private static bool CanPlaceInAirIfNotVoid(Voxel oldVoxel, Voxel newVoxel) => oldVoxel.IsAir && !newVoxel.IsVoid;
}