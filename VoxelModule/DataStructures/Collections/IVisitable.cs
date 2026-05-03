namespace VoxelModule.DataStructures.Collections;

public interface IVisitable<T>
{
    void Accept(T visitor);
}
