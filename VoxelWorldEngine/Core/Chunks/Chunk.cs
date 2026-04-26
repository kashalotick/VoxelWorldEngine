using System.Numerics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Commands;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.Core.Serialization;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Collections.Meshes;
using VoxelWorldEngine.DataStructures.Special.Collections.VoxelTrees;
using VoxelWorldEngine.DataStructures.Special.Structures.Voxels;
using VoxelWorldEngine.Utils;

namespace VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

// TODO: implement logic
public class Chunk : IWorldRegion
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


    public static int ChunkSize => Constants.ChunkSize;

    public static Vector3Int ChunkToGlobal(Vector3Int position)
    {
        return position * ChunkSize;
    }

    public static Vector3Int GlobalToChunk(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.FloorDiv(position.X, ChunkSize),
            MathHelper.FloorDiv(position.Y, ChunkSize),
            MathHelper.FloorDiv(position.Z, ChunkSize)
        );
    }

    public static Vector3Int GlobalToLocal(Vector3Int position)
    {
        return new Vector3Int(
            MathHelper.Mod(position.X, ChunkSize),
            MathHelper.Mod(position.Y, ChunkSize),
            MathHelper.Mod(position.Z, ChunkSize)
        );
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
        var localRay = ray with { Origin = ray.Origin - (Vector3)GlobalPosition };

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

        if (isDataChanged)
        {
            MarkDirty();
        }

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

        MarkDirty();
    }
}