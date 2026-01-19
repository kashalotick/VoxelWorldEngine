namespace VoxelWorldEngine.DataStructures.Octree;

public class OctreeNode
{
    private List<IOctreeNode> _children;
    public NodeData Data { get; set; }
}