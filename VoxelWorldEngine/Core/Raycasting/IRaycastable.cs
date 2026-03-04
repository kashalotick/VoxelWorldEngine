namespace VoxelWorldEngine.Core.Raycasting;

public interface IRaycastable
{
    RayHit Raycast(Ray ray);
}