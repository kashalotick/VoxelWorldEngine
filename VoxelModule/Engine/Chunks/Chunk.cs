using System.Numerics;
using VoxelModule.Core;
using VoxelModule.Core.Raycasting;
using VoxelModule.Core.Serialization;
using VoxelModule.Engine.Octree;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine.Chunks;

public partial class Chunk : IWorldRegion
{
    public bool IsDirty { get; private set; } = false;
    public ChunkMeshData ChunkMesh { get; set; }
    public VoxelOctree Octree { get; set; }

    public Vector3Int Position
    {
        get;
        set
        {
            field = value;
            GlobalPosition = Chunk.ChunkToGlobal(Position);
        }
    }

    public Vector3Int GlobalPosition { get; private set; }


    public Chunk(Vector3Int position)
    {
        Position = position;
    }

    public void MarkDirty()
    {
        IsDirty = true;
    }

    public void MarkClean()
    {
        IsDirty = false;
    }




    public ChunkMemento Save()
    {
        var memento = new ChunkMemento(Position, Octree.Save().Nodes);
        MarkClean();
        return memento;
    }

    public void Restore(ChunkMemento memento)
    {
        Position = memento.Position;
        Octree = new VoxelOctree();
        Octree.Restore(new OctreeMemento<Voxel>(memento.Nodes));
    }

    public RayHit Raycast(Ray ray)
    {
        var localRay = ray;
        localRay.Origin = ray.Origin - (Vector3)GlobalPosition;

        var hit = Octree.Raycast(localRay);

        if (!hit.Voxel.IsAir)
        {
            hit.HitIn += (Vector3)GlobalPosition;
            hit.HitOut += (Vector3)GlobalPosition;
        }

        return hit;
    }

    public bool SetBlock(Vector3Int voxelPositionIndex, BlockId blockId, Func<Voxel, Voxel, bool>? canReplace)
    {
        var chunkPos = Chunk.GlobalToChunk(voxelPositionIndex);
        if (chunkPos != Position) return false;

        var localVoxelIndex = Chunk.GlobalToLocal(voxelPositionIndex);
        var isDataChanged = Octree.SetBlock(localVoxelIndex, blockId, canReplace);

        return isDataChanged;
    }
}