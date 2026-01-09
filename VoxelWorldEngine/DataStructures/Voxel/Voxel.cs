namespace VoxelWorldEngine.DataStructures.Voxel;

public struct Voxel
{
    public byte Density;
    
    
    // TODO: make equality operation
    
    public Voxel(byte density)
    {
        Density = density;
    }
}