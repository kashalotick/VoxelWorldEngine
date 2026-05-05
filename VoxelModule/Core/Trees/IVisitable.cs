namespace VoxelModule.Core.Trees;

public interface IVisitable<T>
{
    void Accept(IOctreeVisitor<T> visitor);
}
