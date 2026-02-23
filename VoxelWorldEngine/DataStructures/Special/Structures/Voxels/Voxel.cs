namespace VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

public struct Voxel
{
    public sbyte Density;
    
    
    // TODO: make equality operation
    
    public Voxel(sbyte density)
    {
        Density = density;
    }
    
    public bool IsEmpty => Density == 0;

}