namespace VoxelWorldEngine.DataStructures.LinearOctree;

public struct ExtendedLinearOctreeNode
{
    public LinearOctreeNode Node;
    public int NodeIndex;
    public int Depth;
    public Vector3Int.Vector3Int Position;
    public int Size => 1 << Depth;

    public ExtendedLinearOctreeNode(LinearOctreeNode node, int nodeIndex, int depth, Vector3Int.Vector3Int position)
    {
        Node = node;
        NodeIndex = nodeIndex;
        Depth = depth;
        Position = position;
    }
}