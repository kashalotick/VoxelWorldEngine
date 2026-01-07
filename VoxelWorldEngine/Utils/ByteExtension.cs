namespace VoxelWorldEngine.Utils;

public static class ByteExtension
{
    static int PopCount(this byte number)
    {
        int count = 0;

        for (byte temp = number; temp > 0; temp &= (byte)(temp - 1))
            count++;
        return count;
    }
}