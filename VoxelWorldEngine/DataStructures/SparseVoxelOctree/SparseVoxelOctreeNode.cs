namespace VoxelWorldEngine.DataStructures.SparseVoxelOctree;

/// <summary>
///     Represents a node within a Sparse Voxel Octree structure. This structure is designed
///     for efficient spatial representation and querying of sparse voxel data.
/// </summary>
public struct SparseVoxelOctreeNode
{
    /// <summary>
    ///     Voxel value.
    /// </summary>
    public Core.Voxels.Voxel Voxel;

    /// <summary>
    ///     Shows which children are not empty. One bit per child. Example for leaf 0b_0000_0000 - no children.
    /// </summary>
    public byte ChildrenMask;

    /// <summary>
    ///     Start index of children in SparseVoxelOctree <see cref="SparseVoxelOctree._nodes" />.
    /// </summary>
    public int ChildrenStartIndex;

    
    /// <summary>
    ///     Returns true if this node is a leaf.
    /// </summary>
    public bool IsLeaf => ChildrenMask == 0;

    /// <summary>
    ///     Returns number of non-empty children.
    /// </summary>
    public int Count => byte.PopCount(ChildrenMask);
    
    
    /// <summary>
    ///     Returns offset of child with given octant index.
    /// </summary>
    /// <param name="octantIndex">Index of the octant 0-7.</param>
    /// <returns>
    ///     0-7 - if child does exist;
    ///     -1 - if child does not exist.
    /// </returns>
    public int GetChildOffset(int octantIndex)
    {
        var octantBit = 1 << octantIndex;
        if ((ChildrenMask & octantBit) == 0) return -1;

        var lowerBitMask = octantBit - 1;
        var offset = byte.PopCount((byte)(lowerBitMask & ChildrenStartIndex));

        return offset;
    }

    /// <summary>
    ///     Retrieves the index of the child node corresponding to the specified octant index.
    /// </summary>
    /// <param name="octantIndex">Index of the octant 0-7.</param>
    /// <returns>
    ///     Index of child in SparseVoxelOctree <see cref="SparseVoxelOctree._nodes" /> if the child exists;
    ///     -1 if the child does not exist.
    /// </returns>
    public int GetChildIndex(int octantIndex)
    {
        var offset = GetChildOffset(octantIndex);
        var index = offset + ChildrenStartIndex;

        return offset >= 0 ? index : -1;
    }


    /// <summary>
    ///     Add the child node at the given octant index.
    /// </summary>
    /// <param name="octantIndex">Index of the octant 0-7.</param>
    public void AddChild(int octantIndex)
    {
        var octantBit = 1 << octantIndex;
        ChildrenMask = (byte)(ChildrenMask | octantBit);
    }

    /// <summary>
    ///     Removes the child node at the given octant index.
    /// </summary>
    /// <param name="octantIndex">Index of the octant 0-7.</param>
    public void RemoveChild(int octantIndex)
    {
        var octantBit = 1 << octantIndex;
        var mask = ~octantBit;
        ChildrenMask = (byte)(ChildrenMask & mask);
    }


    // public bool ShouldSubdivide()
    // {
    //     throw new NotImplementedException(); // TODO: implement
    // }
    // public void Subdivide()
    // {
    //     throw new NotImplementedException(); // TODO: implement
    // }
    // public void Unsubdivide()
    // {
    //     throw new NotImplementedException(); // TODO: implement
    // }
}