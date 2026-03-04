using System.Numerics;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Raycasting;

public struct RayHit
{
    public Vector3 HitIn;
    public Vector3 HitOut;
    public Voxel Voxel;
    public float Distance => (HitOut - HitIn).Length();
    public bool IsHit => !Voxel.IsEmpty;
}