namespace VoxelWorldEngine.Utils;

/// <summary>
///     Provides utility methods for mathematical operations such as floor division
///     and modulus with consistent behavior for negative values.
/// </summary>
public static class MathHelper
{
    /// <summary>
    ///     Computes the floor division of two integers, ensuring consistent behavior for negative values.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The quotient of the floor division.</returns>
    public static int FloorDiv(int a, int b)
    {
        int result = a / b;
        if ((a < 0) != (b < 0) && (a % b != 0))
            result--;
        return result;
    }

    /// <summary>
    ///     Calculates the modulus of two integers, ensuring the result is always positive.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The positive remainder of the division.</returns>
    public static int Mod(int a, int b)
    {
        int result = a % b;
        if (result < 0)
            result += b;
        return result;
    }
}