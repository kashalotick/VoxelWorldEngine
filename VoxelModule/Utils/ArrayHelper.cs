using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Utils;

public static class ArrayHelper
{
    public static Vector3Int GetArraySize<T>(T[,,] array3D) {
        return new Vector3Int(
            array3D.GetLength(2), // X = 5
            array3D.GetLength(0), // Y = 7
            array3D.GetLength(1)  // Z = 5
        );
    }

    public static T?[] Flatten3DArray<T>(T?[,,] array3D) {
        var size = GetArraySize(array3D);
        T?[] flatArray = new T?[size.X * size.Y * size.Z];
        for (int z = 0; z < size.Z; z++)
        for (int y = 0; y < size.Y; y++)
        for (int x = 0; x < size.X; x++)
            // Індекс має бути: X + Y*Width + Z*Width*Height
            flatArray[x + y * size.X + z * size.X * size.Y] = array3D[y, z, x];
        return flatArray;
    }
}