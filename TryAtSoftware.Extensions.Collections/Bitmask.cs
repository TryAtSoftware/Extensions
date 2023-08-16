namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Text;
using TryAtSoftware.Extensions.Collections.Internal;

public class Bitmask
#if NET7_0_OR_GREATER
    : System.Numerics.IBitwiseOperators<Bitmask, Bitmask, Bitmask>
#endif
{
    private const ulong ZeroSegment = 0;
    private const ulong OneSegment = ~ZeroSegment;

    public const int BitsPerSegment = 64;
    private readonly List<ulong> _segments;
    private readonly ulong _lastSegmentMask = OneSegment;

    public int Length { get; }
    private int SegmentsCount => this._segments.Count;

    public Bitmask(int length, bool initializeWithZeros)
    {
        var requiredSegmentsCount = Math.DivRem(length, BitsPerSegment, out var remainder);

        if (remainder != 0)
        {
            requiredSegmentsCount++;
            this._lastSegmentMask <<= (BitsPerSegment - remainder);
        }

        this._segments = new List<ulong>(capacity: requiredSegmentsCount);

        this.Length = length;

        var filler = initializeWithZeros ? ZeroSegment : OneSegment;
        for (var i = 0; i < requiredSegmentsCount; i++) this._segments.Add(filler);
        this.NormalizeLastSegment();
    }

    public void Set(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        this._segments[segmentIndex] |= (1UL << bitIndex);
    }

    public void Unset(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        this._segments[segmentIndex] &= ~(1UL << bitIndex);
    }

    public bool IsSet(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        return (this._segments[segmentIndex] & (1UL << bitIndex)) != 0;
    }

    public int FindLeastSignificantNonZeroBit()
    {
        for (var i = this._segments.Count - 1; i >= 0; i--)
        {
            var currentSegment = this._segments[i];
            if (currentSegment == ZeroSegment) continue;

            return (this._segments.Count - (i + 1)) * BitsPerSegment + Bits.TrailingZeroCount(currentSegment);
        }

        return -1;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < this.Length; i++) sb.Append(this.IsSet(i) ? '1' : '0');

        return sb.ToString();
    }

    public static Bitmask operator &(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseAnd);

    public static Bitmask operator |(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseOr);

    public static Bitmask operator ^(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseXor);

    public static Bitmask operator ~(Bitmask a)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));

        var result = new Bitmask(a.Length, initializeWithZeros: true);
        for (var i = 0; i < a.SegmentsCount; i++) result.SetSegment(i, ~a.GetSegment(i));

        return result;
    }

    private static ulong BitwiseAnd(ulong a, ulong b) => a & b;
    private static ulong BitwiseOr(ulong a, ulong b) => a | b;
    private static ulong BitwiseXor(ulong a, ulong b) => a ^ b;

    private static Bitmask ExecuteBitwiseOperation(Bitmask a, Bitmask b, Func<ulong, ulong, ulong> operation)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));

        var result = new Bitmask(length: Math.Max(a.Length, b.Length), initializeWithZeros: false);
        for (var i = 0; i < result._segments.Count; i++)
        {
            var left = i < a.SegmentsCount ? a.GetSegment(i) : 0;
            var right = i < b.SegmentsCount ? b.GetSegment(i) : 0;

            result.SetSegment(i, operation(left, right));
        }

        return result;
    }

    private void SetSegment(int index, ulong value)
    {
        this.ValidateSegmentIndex(index);

        this._segments[index] = value;
        if (index == this.SegmentsCount - 1) this.NormalizeLastSegment();
    }

    private ulong GetSegment(int index)
    {
        this.ValidateSegmentIndex(index);
        return this._segments[index];
    }

    private (int SegmentIndex, int BitIndex) Locate(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be a non-negative number.");
        if (position >= this.Length) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be less than the total number of bits.");

        var segmentIndex = Math.DivRem(position, BitsPerSegment, out var bitIndex);
        return (segmentIndex, BitsPerSegment - (bitIndex + 1));
    }

    private void ValidateSegmentIndex(int segmentIndex)
    {
        if (segmentIndex < 0) throw new ArgumentOutOfRangeException(nameof(segmentIndex), "Segment index must be a non-negative number.");
        if (segmentIndex >= this.SegmentsCount) throw new ArgumentOutOfRangeException(nameof(segmentIndex), "Segment index must be less than the total number of segments.");
    }

    private void NormalizeLastSegment() => this._segments[^1] &= this._lastSegmentMask;
}