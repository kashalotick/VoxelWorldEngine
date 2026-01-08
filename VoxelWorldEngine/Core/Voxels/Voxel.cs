namespace VoxelWorldEngine.Core.Voxels;

public struct Voxel
{
    public Material Material;
    public byte Density;


    public Voxel Air => new(Material.Air, 0);
    public Voxel Mixed => new(Material.Mixed, 0);

    public Voxel(Material material, byte density)
    {
        Material = material;
        Density = density;
    }
}