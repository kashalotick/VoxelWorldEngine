using VoxelWorldEngine.DataStructures.Octree.SparseOctree;

namespace VoxelWorldEngine.DataStructures.Octree;

public interface IOctreeNode<T>
{
    int MaxDepth { get; }
    int Depth { get; set; }
    bool IsLeaf { get; }
    
    T Data { get; set; }



    
    void Insert(IOctreeNode<T> node);
    void Apply(Bound region, Action<T> operation);

    List<IOctreeNode<T>> Query(Bound region);
    
    void Split();
    void TryMerge();
    
    // void Merge(); // hard merge for LOD




    // IOctreeNode<T> FindNode();
    //
    // IOctreeNode<T> FindFirstLeaf(Vector3Int.Vector3Int position);
    // // void SetFirstLeafData(Vector3Int.Vector3Int position, NodeData data);




}

// TODO: separate to IN and OUT interface