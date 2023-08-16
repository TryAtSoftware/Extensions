namespace TryAtSoftware.Extensions.Collections.Internal;

internal static class Bits
{
#if NET7_0_OR_GREATER
    public static int TrailingZeroCount(ulong segment) => System.Numerics.BitOperations.TrailingZeroCount(segment);
#else
    private const ulong TrailingZeroCountDeBruijnConstant = 0x077CB531UL;

    private static readonly int[] _trailingZeroCountDeBruijnTable =
    {
        0, 1, 28, 2, 29, 14, 24, 3, 30, 22,
        20, 15, 25, 17, 4, 8, 31, 27, 13, 23,
        21, 19, 16, 7, 26, 12, 18, 6, 11, 5,
        10, 9
    };

    public static int TrailingZeroCount(ulong segment)
    {
        var deBruijnIndex = (int)((segment & ~(segment - 1) * TrailingZeroCountDeBruijnConstant) >> 27);
        return _trailingZeroCountDeBruijnTable[deBruijnIndex];
    }
#endif
}