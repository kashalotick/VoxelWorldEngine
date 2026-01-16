namespace VoxelWorldEngine.DataStructures;

public struct MinMaxAvg<T>
{
    public T Min;
    public T Max;
    public T Avg;

    public MinMaxAvg(T min, T max, T avg)
    {
        Min = min;
        Max = max;
        Avg = avg;
    }
}