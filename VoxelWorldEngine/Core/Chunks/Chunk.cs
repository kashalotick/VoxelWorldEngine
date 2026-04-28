using System.Numerics;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.Core.Serialization;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;

namespace VoxelWorldEngine.Core.Chunks;

public partial class Chunk : IWorldRegion
{
    public bool IsDirty { get; private set; } = false;
    public VoxelOctree Octree { get; set; }

    public Vector3Int Position
    {
        get;
        set
        {
            field = value;
            GlobalPosition = ChunkToGlobal(Position);
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
        var memento = new ChunkMemento(Position, Octree.Save());
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

    public bool PlaceBlock(Vector3Int voxelPositionIndex, BlockId blockId)
    {
        var localVoxelIndex = GlobalToLocal(voxelPositionIndex);
        var isDataChanged = Octree.PlaceBlock(localVoxelIndex, blockId);

        return isDataChanged;
    }

    public void ModifyArea(
        Vector3Int insertPosition,
        Vector3Int areaSize,
        Voxel[] data,
        Func<Voxel, Voxel, bool>? canReplace
    )
    {
        var chunkOriginGlobal = ChunkToGlobal(Position);
        var relativeInsertPosition = insertPosition - chunkOriginGlobal;

        Octree.ModifyArea(relativeInsertPosition, areaSize, data, canReplace);

    }
}