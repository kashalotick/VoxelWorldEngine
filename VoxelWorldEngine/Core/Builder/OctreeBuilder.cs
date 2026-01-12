using VoxelWorldEngine.Core.Generator;
using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.DataStructures.Voxel;

namespace VoxelWorldEngine.Core.Builder;

public class OctreeBuilder
{
    private DensityGenerator _densityGenerator;

    public OctreeBuilder()
    {
        _densityGenerator = new DensityGenerator();
    }


    public LinearOctree Build(Chunk chunk)
    {
        var octree = new LinearOctree();

        var indent = LinearOctree.MaxDepth;
        var min = ChunkMath.ChunkToGlobal(chunk.Position);
        
        var nodeSize = 1 << indent;
        var max = min + Vector3Int.One * nodeSize;

        var nodeIndex = octree.RootIndex;
        if (ShouldSubdivideRegion(min, max) && nodeSize > 1)
        {
            nodeSize >>= 1;
            var firstChildIndex = octree.SubdivideNode(nodeIndex);
            for (int i = 0; i < 8; i++)
            {
                min = min;
                max = min + Vector3Int.One * nodeSize;
                // var childMin
                // var childMax
            }
        }
        else
        {
            ProcessLeaf(min, max, octree, nodeIndex);
        }
            
        return octree;
    }

    private void ProcessLeaf(Vector3Int min, Vector3Int max, LinearOctree octree, int nodeIndex)
    {
        // TODO: remaker with avg density ???
        var center = (min + max) / 2;
        sbyte density = _densityGenerator.GetValue(center);
            
        var voxel = new Voxel(density);
        octree.SetNodeVoxel(nodeIndex, voxel);
    }


    private void Subdivide()
    {
        throw new NotImplementedException();
    }

    private bool ShouldSubdivideRegion(Vector3Int a, Vector3Int b)
    {
        var regionSize = Vector3Int.Abs(a - b).X + 1;
        var indent = int.Log2(regionSize);
        // in indent?
        // out density?
        

        var isHereSurface = _densityGenerator.IsHereAnySurface(a, b);
        
        return isHereSurface;
    }
}