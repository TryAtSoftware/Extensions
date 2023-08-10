namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;

public class Bitmask
{
    public const int BitsPerSegment = 64;
    private readonly List<long> _segments;

    public int Count { get; }
    public int SegmentsCount => this._segments.Count;
    public bool IsZero { get; }
    public bool IsOne { get; }

    public Bitmask(int count, bool initializeWithZeros)
    {
        var requiredSegmentsCount = Math.DivRem(count, BitsPerSegment, out var remainder);
        if (remainder != 0) requiredSegmentsCount++;

        this._segments = new List<long>(capacity: requiredSegmentsCount);

        var filler = 0;
        if (!initializeWithZeros) filler = ~filler;

        for (var i = 0; i < requiredSegmentsCount; i++) this._segments.Add(filler);

        this.Count = count;
        this.IsZero = initializeWithZeros;
        this.IsOne = !initializeWithZeros;
    }

    public bool IsSet(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        return (this._segments[segmentIndex] & (1 << bitIndex)) != 0;
    }

    public void SetSegment(int index, long value)
    {
        this.ValidateSegmentIndex(index);
        this._segments[index] = value;
    }

    public long GetSegment(int index)
    {
        this.ValidateSegmentIndex(index);
        return this._segments[index];
    }

    public static Bitmask operator &(Bitmask a, Bitmask b)
    {
        var result = new Bitmask(count: Math.Max(a.Count, b.Count), initializeWithZeros: false);
        for (var i = 0; i < result._segments.Count; i++)
        {
            var segment = result.GetSegment(i);

            if (i < a.SegmentsCount) segment &= a.GetSegment(i);
            if (i < b.SegmentsCount) segment &= b.GetSegment(i);

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