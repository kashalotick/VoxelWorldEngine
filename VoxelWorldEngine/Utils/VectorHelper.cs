using System.Numerics;
using VoxelWorldEngine.DataStructures.Vector3Int;

namespace VoxelWorldEngine.Utils;

// TODO: tests
public static class VectorHelper
{
    public static Vector3Int Transform(Vector3Int vector, Matrix4x4 matrix)
    {
        var vec3 = vector.ToVector3();
        var transformed = Vector3.Transform(vec3, matrix);
        var result = transformed.ToVector3Int();

        return result;
    }

    public static Matrix4x4 GetTransformMatrix(Vector3Int newOrigin, Vector3Int newZAxis,
        Vector3Int oldZAxis = default, Vector3Int oldXAxis = default)
    {
        return GetTransformMatrix(newOrigin.ToVector3(), newZAxis.ToVector3(), oldZAxis.ToVector3(),
            oldXAxis.ToVector3());
    }

    public static Matrix4x4 GetTransformMatrix(
        Vector3 newOrigin, Vector3 newZAxis, 
        Vector3 oldZAxis = default, Vector3 oldXAxis = default
        ) 
    {
        if (oldZAxis == default) oldZAxis = Vector3.UnitZ;
        if (oldXAxis == default) oldXAxis = Vector3.UnitX;

        Vector3 newXAxis;
        
        if (newZAxis == oldZAxis) newXAxis = oldXAxis;
        else if (newZAxis == -oldZAxis) newXAxis = -oldXAxis;
        else newXAxis = Vector3.Cross(oldZAxis, newZAxis);
        
        var result  = new Matrix4x4();
        
        var axisZ = Vector3.Normalize(newZAxis);
        var axisX = Vector3.Normalize(newXAxis);
        var axisY = Vector3.Cross(axisZ, axisX);
        
        result.X = axisX.AsVector4();
        result.Y = axisY.AsVector4();
        result.Z = axisZ.AsVector4();
        result.W = Vector4.Create(newOrigin, 1);

        return result;
    }
}