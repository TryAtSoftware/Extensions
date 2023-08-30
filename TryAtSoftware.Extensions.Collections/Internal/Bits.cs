namespace TryAtSoftware.Extensions.Collections.Internal;

internal static class Bits
{
#if NET7_0_OR_GREATER
    public static int TrailingZeroCount(ulong segment) => System.Numerics.BitOperations.TrailingZeroCount(segment);

    public static int LeadingZeroCount(ulong segment) => System.Numerics.BitOperations.LeadingZeroCount(segment);
#else
    private const uint TrailingZeroCountDeBruijnConstant = 0x077CB531U;
    private const uint Log2DeBruijnConstant = 0x07C4ACDDU;

    private static readonly int[] _trailingZeroCountDeBruijnTable =
    {
        0, 1, 28, 2, 29, 14, 24, 3, 30, 22,
        20, 15, 25, 17, 4, 8, 31, 27, 13, 23,
        21, 19, 16, 7, 26, 12, 18, 6, 11, 5,
        10, 9
    };


    private static readonly int[] _log2DeBruijnTable =
    {
        0, 9, 1, 10, 13, 21, 2, 29, 11, 14,
        16, 18, 22, 25, 3, 30, 8, 12, 20, 28,
        15, 17, 24, 07, 19, 27, 23, 6, 26, 5,
        4, 31
    };

    public static int TrailingZeroCount(ulong value)
    {
        var (lo, offset) = SplitValueLow(value);
        return offset + TrailingZeroCount(lo);
    }

    public static int TrailingZeroCount(uint value)
    {
        var deBruijnIndex = ((value & ~(value - 1)) * TrailingZeroCountDeBruijnConstant) >> 27;
        return _trailingZeroCountDeBruijnTable[deBruijnIndex];
    }

    public static int LeadingZeroCount(ulong value)
    {
        var (hi, offset) = SplitValueHigh(value);
        return offset + LeadingZeroCount(hi);
    }

    public static int LeadingZeroCount(uint value)
    {
        var log = Log2(value);

        // This is equivalent to 31 - Log2(value) since the log value is always in range [0, 31]
        return 31 ^ log;
    }

    public static int Log2(ulong value)
    {
        var (hi, offset) = SplitValueHigh(value);
        return offset + Log2(hi);
    }
    
    public static int Log2(uint value)
    {
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;
        
        var deBruijnIndex = (value * Log2DeBruijnConstant) >> 27;
        return _log2DeBruijnTable[deBruijnIndex];
    }

    private static (uint Value, int Offset) SplitValueHigh(ulong value)
    {
        var hi = (uint)(value >> 32);

        var offset = 0;
        if (hi == 0)
        {
            offset += 32;
            hi = (uint)(value);
        }

        return (hi, offset);
    }

    private static (uint Value, int Offset) SplitValueLow(ulong value)
    {
        var lo = (uint)value;

        var offset = 0;
        if (lo == 0)
        {
            offset += 32;
            lo = (uint)(value >> 32);
        }

        return (lo, offset);
    }
#endif
}