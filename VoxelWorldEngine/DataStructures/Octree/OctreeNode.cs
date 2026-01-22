namespace VoxelWorldEngine.DataStructures.Octree;

public class OctreeNode : IOctreeNode
{
    private IOctreeNode?[] _children;
    private int count;
    
    public bool IsLeaf => count == 0;
    
    public NodeData Data { get; set; }

    public int MaxDepth = 8;
    public int Depth;
    public int Size => 1 << (MaxDepth - Depth);
    
    public Vector3Int.Vector3Int Position;
    public Vector3Int.Vector3Int Min => Position;
    public Vector3Int.Vector3Int Max => Position + Vector3Int.Vector3Int.One * (Size - 1);
    
    // public int Count => _children.Length;
    
    
    
    
    public IOctreeNode FindFirstLeaf(Vector3Int.Vector3Int position)
    {
        if (IsLeaf)
        {
            return this;
        }
        
        var childIndex = GetOctant(position, Size);
        var child = GetChild(childIndex);

        if (child != null)
        {
            var newPosition = position - GetOctantCorner(childIndex, Size);
            return child.FindFirstLeaf(newPosition);
        }

        return this;
    }

    public void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData data)
    {
        var node = FindFirstLeaf(position);
        node.Data = data;
    }



    public IOctreeNode? GetChild(int octant)
    {
        return _children[octant];
    }

    public void SetData(NodeData data)
    {
        Data = data;
    }
    
    public void AddChild(int octant, NodeData data)
    {
        count++;

        // if (Depth + 1 == MaxDepth)
        // {
        //     var child = new OctreeLeaf();
        // }
        var child = new OctreeNode()
        {
            Depth = Depth + 1,
            Position = Position + GetOctantCorner(octant, Size),
            Data = data
        };
        _children[octant] = child;

    }
    public void RemoveChild(int octant)
    {
        count--;
        // Dispose ????
        _children[octant] = null;
    }

    public static OctreeNode CreateRoot(NodeData data)
    {
        var root = new OctreeNode()
        {
            Depth = 1,
            Position = Vector3Int.Vector3Int.Zero,
        };

        return root;
    }
    
    
    
    public static Vector3Int.Vector3Int GetOctantCorner(int octant, int size)
    {
        var halfSize = size >> 1;
        
        return new Vector3Int.Vector3Int(
            (octant & 1) != 0 ? halfSize : 0, // x
            (octant & 2) != 0 ? halfSize : 0, // y
            (octant & 4) != 0 ? halfSize : 0  // z
        );
    }
    public static int GetOctant(Vector3Int.Vector3Int position, int size)
    {
        var halfSize = size >> 1;
        var octant = 0;
        
        if (position.X >= halfSize) octant |= 1;
        if (position.Y >= halfSize) octant |= 2;
        if (position.Z >= halfSize) octant |= 4;
        
        return octant;
    }
}