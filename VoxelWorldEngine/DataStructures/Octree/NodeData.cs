using System.Drawing;

namespace VoxelWorldEngine.DataStructures.Octree;

public struct NodeData
{
    public Voxel.Voxel Voxel;
    public int Depth;
    public int Size => 1 << Depth;
    public Bounding BoundingBox;
}

public struct Bounding
{
    public Vector3Int.Vector3Int Min;
    public Vector3Int.Vector3Int Max;

    public Bounding(Vector3Int.Vector3Int min)
    {
        Min = min;
    }
}