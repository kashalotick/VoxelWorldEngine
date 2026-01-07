namespace VoxelWorldEngine.Core.Voxels;

public struct Voxel
{
    public byte Density;
    public Material Material;

    public Voxel(byte density, Material material)
    {
        Density = density;
        Material = material;
    }
    public Voxel()
    {
        Density = 0;
        Material = Material.Air;
    }
}