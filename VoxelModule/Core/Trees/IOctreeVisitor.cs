namespace VoxelModule.Core.Trees;

public interface IOctreeVisitor<T>
{
    void Visit(IOctreeNodeReadonly<T> tree);
}
