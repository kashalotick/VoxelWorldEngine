using System.Numerics;
using VoxelModule.Engine.Octree;

namespace VoxelModule.Core.Raycasting;

public struct RayHit
{
    public Vector3 HitIn;
    public Vector3 HitOut;
    public Vector3 HitFaceNormal;
    public Voxel Voxel;
    public float Distance => (HitOut - HitIn).Length();
    public bool IsHit => !Voxel.IsAir;


    public static RayHit NoHit => new RayHit { Voxel = Voxel.Air };
    
    
    public override string ToString()
    {
        return $"RayHit | HitIn: {HitIn}, HitOut: {HitOut}, Normal: {HitFaceNormal}, " +
               $"Voxel: {Voxel}, Distance: {Distance}, IsHit: {IsHit}";
    }
    
}