namespace VoxelModule.Core.Raycasting;

public interface IRaycastable
{
    RayHit Raycast(Ray ray);
}