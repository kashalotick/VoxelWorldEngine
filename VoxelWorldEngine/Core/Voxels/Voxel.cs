namespace VoxelWorldEngine.Core.Voxels;

public struct Voxel
{
    public Material Material;
    public byte Density;


    
    public Voxel Air => new Voxel(Material.Air, 0);
    public Voxel Mixed => new Voxel(Material.Mixed, 0);
    
    public Voxel(Material material, byte density)
    {
        Material = material;
        Density = density;
    }


}