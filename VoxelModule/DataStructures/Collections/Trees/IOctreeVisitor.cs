namespace VoxelModule.DataStructures.Collections.Trees;

public interface IOctreeVisitor<T>
{
    void Visit(IOctreeNodeReadonly<T> tree);
}
