using System.Numerics;
using VoxelModule.DataStructures.Special.Structures.Voxels;

namespace VoxelModule.DataStructures.Special.Structures.Vertices;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
public struct ChunkVertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 Uv;
    public BlockId BlockId;
}