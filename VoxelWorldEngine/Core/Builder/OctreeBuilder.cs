using VoxelWorldEngine.Core.Generator;
using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Vector3Int;

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
            
            
        return octree;
    }

    private void Subdivide()
    {
        throw new NotImplementedException();

    }

    private bool ShouldSubdivide(Vector3Int a, Vector3Int b)
    {
        // in indent?
        // out density?
        

        var isHereSurface = _densityGenerator.IsHereAnySurface(a, b);
        
        return isHereSurface;
    }
}