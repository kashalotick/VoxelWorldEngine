using VoxelWorldEngine.Core.Generators.Interfaces;
using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Vector3Int;
using VoxelWorldEngine.DataStructures.Voxel;

namespace VoxelWorldEngine.Core.Builders;

// TODO: make tests
// TODO: add Observer class + observer field + distance check
/// <summary>
///     The OctreeBuilder class provides functionality to construct a LinearOctree structure
///     using density data generated within a specified chunk. It acts as a bridge between
///     the chunk-based representation and the octree-based representation of a voxel world.
/// </summary>
public class OctreeBuilder
{
    private readonly IScalarFieldGenerator<Vector3Int, sbyte> _densityGenerator;

    public OctreeBuilder(IScalarFieldGenerator<Vector3Int, sbyte> densityGenerator)
    {
        _densityGenerator = densityGenerator;
    }


    /// <summary>
    ///     Constructs a linear octree from the density data of the provided chunk.
    /// </summary>
    /// <param name="chunk">The chunk, from which the octree will be constructed.</param>
    /// <returns>A LinearOctree instance.</returns>
    public LinearOctree Build(Chunk chunk)
    {
        var octree = new LinearOctree();

        var nodeSize = 1 << LinearOctree.MaxDepth;
        var parentMin = ChunkMath.ChunkToGlobal(chunk.Position);

        ProcessNode(octree.RootIndex, nodeSize, octree, parentMin);

        return octree;
    }

    /// <summary>
    ///     Processing node on a specified index.
    /// </summary>
    /// <param name="nodeIndex">Node index in node list.</param>
    /// <param name="nodeSize">Node size - power of 2.</param>
    /// <param name="octree">Building octree.</param>
    /// <param name="octantCorner">Position of 0's corner of octant.</param>
    private void ProcessNode(int nodeIndex, int nodeSize, LinearOctree octree, Vector3Int octantCorner)
    {
        if (ShouldSubdivideNode(nodeSize, octantCorner) && nodeSize > 1)
        {
            ProcessSubdivide(nodeIndex, nodeSize, octree, octantCorner);
        }
        else
        {
            ProcessLeaf(nodeIndex, nodeSize, octree, octantCorner);

        }
    }

    /// <summary>
    ///     Processing subdividing of node on a specified index.
    /// </summary>
    /// <param name="nodeIndex">Node index in node list.</param>
    /// <param name="nodeSize">Node size - power of 2.</param>
    /// <param name="octree">Building octree.</param>
    /// <param name="octantCorner">Position of 0's corner of octant.</param>
    private void ProcessSubdivide(int nodeIndex, int nodeSize, LinearOctree octree, Vector3Int octantCorner)
    {
        var firstChildIndex = octree.SubdivideNode(nodeIndex);
        var childNodeSize = nodeSize >> 1;

        for (var i = 0; i < 8; i++)
        {
            var octantLocalPosition = OctreeMath.GetOctantCorner(i, nodeSize);
            var childOctantCorner = octantCorner + octantLocalPosition;

            var childNodeIndex = firstChildIndex + i;
            ProcessNode(childNodeIndex, childNodeSize, octree, childOctantCorner);
        }
    }

    /// <summary>
    ///     Processing leaf node on a specified index.
    /// </summary>
    /// <param name="nodeIndex">Node index in node list.</param>
    /// <param name="nodeSize">Node size - power of 2.</param>
    /// <param name="octree">Building octree.</param>
    /// <param name="octantCorner">Position of 0's corner of octant.</param>
    private void ProcessLeaf(int nodeIndex, int nodeSize, LinearOctree octree, Vector3Int octantCorner)
    {
        // TODO: remaker with avg density ???
        var octantLastCorner = octantCorner + Vector3Int.One * nodeSize;

        // var center = nodeSize > 1 ? octantCorner + Vector3Int.One * (nodeSize / 2) : octantCorner;
        var density = _densityGenerator.GetMinMax(octantCorner, octantLastCorner);
        var avg = (density.min + density.max)/2;
        avg = avg == 0 ? sbyte.MinValue : avg;
        var voxel = new Voxel((sbyte)avg);
        octree.SetNodeVoxel(nodeIndex, voxel);
    }

    /// <summary>
    ///     Calculates whether the node should be subdivided or not.
    /// </summary>
    /// <param name="nodeSize">Node size - power of 2.</param>
    /// <param name="octantCorner">Position of 0's corner of octant.</param>
    /// <returns>True if should subdivide.</returns>
    private bool ShouldSubdivideNode(int nodeSize, Vector3Int octantCorner)
    {
        var octantLastCorner = octantCorner + Vector3Int.One * nodeSize;

        if (nodeSize > 1)
        {
            // var indent = int.Log2(regionSize);
            // out density?
            // TODO: add distance to observer check

            var isHereSurface = _densityGenerator.IsHereAnySurface(octantCorner, octantLastCorner);

            return isHereSurface;
        }
        return false;
    }
}