﻿namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;

public class Bitmask
{
    private const int BitsPerElement = 64;
    private readonly List<long> _segments;
    
    public bool IsZero { get; }
    public bool IsOne { get; }

    public Bitmask(int count, bool initializeWithZeros)
    {
        var requiredSegmentsCount = Math.DivRem(count, BitsPerElement, out var remainder);
        if (remainder != 0) requiredSegmentsCount++;
        
        this._segments = new List<long>(capacity: requiredSegmentsCount);

        var filler = 0;
        if (!initializeWithZeros) filler = ~filler;

        for (var i = 0; i < requiredSegmentsCount; i++) this._segments.Add(filler);

        this.IsZero = initializeWithZeros;
        this.IsOne = !initializeWithZeros;
    }

    public bool IsSet(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        return (this._segments[segmentIndex] & (1 << bitIndex)) != 0;
    }

    private (int SegmentIndex, int BitIndex) Locate(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be a non-negative number.");

        var segmentIndex = Math.DivRem(position, BitsPerElement, out var bitIndex);
        if (segmentIndex >= this._segments.Count) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be less than the number of total bits.");

        return (segmentIndex, bitIndex);
    }
}