namespace VoxelWorldEngine.Core.Voxels;

public struct Voxel
{
    public float Density;
    public Material Material;

    public Voxel(float density, Material material)
    {
        Density = density;
        Material = material;
    }
    public Voxel()
    {
        Density = -1;
        Material = Material.Air;
    }
}