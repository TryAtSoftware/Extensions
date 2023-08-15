namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;

public class Bitmask
{
    public const int BitsPerSegment = 64;
    private readonly List<ulong> _segments;

    public int Count { get; }
    public int SegmentsCount => this._segments.Count;
    public bool IsZero { get; }
    public bool IsOne { get; }

    public Bitmask(int count, bool initializeWithZeros)
    {
        var requiredSegmentsCount = Math.DivRem(count, BitsPerSegment, out var remainder);
        if (remainder != 0) requiredSegmentsCount++;

        this._segments = new List<ulong>(capacity: requiredSegmentsCount);

        var filler = 0UL;
        if (!initializeWithZeros) filler = ~filler;

        for (var i = 0; i < requiredSegmentsCount; i++) this._segments.Add(filler);

        this.Count = count;
        this.IsZero = initializeWithZeros;
        this.IsOne = !initializeWithZeros;
    }

    public bool IsSet(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        return (this._segments[segmentIndex] & (1UL << bitIndex)) != 0;
    }

    public void SetSegment(int index, ulong value)
    {
        this.ValidateSegmentIndex(index);
        this._segments[index] = value;
    }

    public ulong GetSegment(int index)
    {
        this.ValidateSegmentIndex(index);
        return this._segments[index];
    }

    public static Bitmask operator &(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseAnd);

    public static Bitmask operator |(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseOr);

    public static Bitmask operator ^(Bitmask a, Bitmask b) => ExecuteBitwiseOperation(a, b, BitwiseXor);

    public static Bitmask operator ~(Bitmask a)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));

        var result = new Bitmask(a.Count, initializeWithZeros: true);
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
        
        var result = new Bitmask(count: Math.Max(a.Count, b.Count), initializeWithZeros: false);
        for (var i = 0; i < result._segments.Count; i++)
        {
            var segment = result.GetSegment(i);

            if (i < a.SegmentsCount) segment = operation(segment, a.GetSegment(i));
            if (i < b.SegmentsCount) segment = operation(segment, b.GetSegment(i));

            result.SetSegment(i, segment);
        }

        return result;
    }

    private (int SegmentIndex, int BitIndex) Locate(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be a non-negative number.");
        if (position >= this.Count) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be less than the total number of bits.");

        var segmentIndex = Math.DivRem(position, BitsPerSegment, out var bitIndex);
        return (segmentIndex, bitIndex);
    }

    private void ValidateSegmentIndex(int segmentIndex)
    {
        if (segmentIndex < 0) throw new ArgumentOutOfRangeException(nameof(segmentIndex), "Segment index must be a non-negative number.");
        if (segmentIndex >= this.SegmentsCount) throw new ArgumentOutOfRangeException(nameof(segmentIndex), "Segment index must be less than the total number of segments.");
    }
}