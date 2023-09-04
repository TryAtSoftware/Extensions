namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Text;
using TryAtSoftware.Extensions.Collections.Internal;

/// <summary>
/// Class representing a bitmask (sequence of bits that can be optimally manipulated).
/// </summary>
public class Bitmask
#if NET7_0_OR_GREATER
    : System.Numerics.IBitwiseOperators<Bitmask, Bitmask, Bitmask>, System.Numerics.IShiftOperators<Bitmask, int, Bitmask>
#endif
{
    private const ulong ZeroSegment = 0;
    private const ulong OneSegment = ~ZeroSegment;
    private const int BitsPerSegment = 64;

    private readonly List<ulong> _segments;
    private readonly ulong _lastSegmentMask = OneSegment;

    /// <summary>
    /// Gets the length of this bitmask.
    /// </summary>
    public int Length { get; }

    private int SegmentsCount => this._segments.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bitmask"/> class.
    /// </summary>
    /// <param name="length">The length of the bitmask.</param>
    /// <param name="initializeWithZeros">A value indicating whether this bitmask should be filled with zeros or ones initially.</param>
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

    /// <summary>
    /// Use this method to set the bit at the requested position.
    /// </summary>
    /// <param name="position">The position of the bit that should be set.</param>
    public void Set(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        this._segments[segmentIndex] |= (1UL << bitIndex);
    }

    /// <summary>
    /// Use this method to set all bits.
    /// </summary>
    public void SetAll()
    {
        for (var i = 0; i < this.SegmentsCount; i++)
            this.SetSegment(i, OneSegment);
    }

    /// <summary>
    /// Use this method to unset the bit at the requested position.
    /// </summary>
    /// <param name="position">The position of the bit that should be unset.</param>
    public void Unset(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        this._segments[segmentIndex] &= ~(1UL << bitIndex);
    }

    /// <summary>
    /// Use this method to unset all bits.
    /// </summary>
    public void UnsetAll()
    {
        for (var i = 0; i < this.SegmentsCount; i++)
            this.SetSegment(i, ZeroSegment);
    }

    /// <summary>
    /// Use this method to check the status of the bit at the requested position.
    /// </summary>
    /// <param name="position">The position of the bit whose status should be checked.</param>
    /// <returns>Returns <c>true</c> if the bit is set. Otherwise, returns <c>false</c>.</returns>
    public bool IsSet(int position)
    {
        var (segmentIndex, bitIndex) = this.Locate(position);
        return (this._segments[segmentIndex] & (1UL << bitIndex)) != 0;
    }
    
    /// <summary>
    /// Use this method to find the position of the least significant (right-most) bit that is set.
    /// </summary>
    /// <returns>Returns the position of the least significant set bit. Returns -1 if there are no set bits.</returns>
    public int FindLeastSignificantSetBit() => this.FindLeastSignificantSetBit(inverse: false);

    /// <summary>
    /// Use this method to find the position of the least significant (right-most) bit that is unset.
    /// </summary>
    /// <returns>Returns the position of the least significant unset bit. Returns -1 if there are no unset bits.</returns>
    public int FindLeastSignificantUnsetBit() => this.FindLeastSignificantSetBit(inverse: true);

    /// <summary>
    /// Use this method to find the position of the most significant (left-most) bit that is set.
    /// </summary>
    /// <returns>Returns the position of the most significant set bit. Returns -1 if there are no set bits.</returns>
    public int FindMostSignificantSetBit() => this.FindMostSignificantSetBit(inverse: false);

    /// <summary>
    /// Use this method to find the position of the most significant (left-most) bit that is unset.
    /// </summary>
    /// <returns>Returns the position of the most significant unset bit. Returns -1 if there are no unset bits.</returns>
    public int FindMostSignificantUnsetBit() => this.FindMostSignificantSetBit(inverse: true);

    public int CountSetBits()
    {
        var ans = 0;
        for (var i = 0; i < this.SegmentsCount; i++) ans += Bits.CountSetBits(this.GetSegment(i));

        return ans;
    }

    public int CountUnsetBits() => this.Length - this.CountSetBits();

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < this.Length; i++) sb.Append(this.IsSet(i) ? '1' : '0');

        return sb.ToString();
    }

    /// <summary>
    /// Computes the bitwise-and of two bitmasks.
    /// </summary>
    /// <param name="left">The first bitmask.</param>
    /// <param name="right">The second bitmask.</param>
    /// <returns>The bitwise-and of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Bitmask operator &(Bitmask left, Bitmask right) => ExecuteBitwiseOperation(left, right, BitwiseAnd);

    /// <summary>
    /// Computes the bitwise-or of two bitmasks.
    /// </summary>
    /// <param name="left">The first bitmask.</param>
    /// <param name="right">The second bitmask.</param>
    /// <returns>The bitwise-or of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Bitmask operator |(Bitmask left, Bitmask right) => ExecuteBitwiseOperation(left, right, BitwiseOr);

    /// <summary>
    /// Computes the exclusive-or of two bitmasks.
    /// </summary>
    /// <param name="left">The first bitmask.</param>
    /// <param name="right">The second bitmask.</param>
    /// <returns>The exclusive-or of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Bitmask operator ^(Bitmask left, Bitmask right) => ExecuteBitwiseOperation(left, right, BitwiseXor);

    /// <summary>
    /// Computes the ones-complement representation of a bitmask.
    /// </summary>
    /// <param name="bitmask">The bitmask for which to compute its ones-complement.</param>
    /// <returns>The ones-complement of <paramref name="bitmask" />.</returns>
    public static Bitmask operator ~(Bitmask bitmask)
    {
        if (bitmask is null) throw new ArgumentNullException(nameof(bitmask));

        var result = new Bitmask(bitmask.Length, initializeWithZeros: true);
        for (var i = 0; i < bitmask.SegmentsCount; i++) result.SetSegment(i, ~bitmask.GetSegment(i));

        return result;
    }

    /// <summary>
    /// Shifts the bitmask left by a given amount.
    /// </summary>
    /// <param name="value">The bitmask which is shifted left by <paramref name="shiftAmount" />.</param>
    /// <param name="shiftAmount">The amount by which <paramref name="value" /> is shifted left.</param>
    /// <returns>The result of shifting <paramref name="value" /> left by <paramref name="shiftAmount" />.</returns>
    public static Bitmask operator <<(Bitmask value, int shiftAmount)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (shiftAmount < 0) throw new ArgumentException("The shift amount must be a non-negative number.", nameof(shiftAmount));
        
        var result = new Bitmask(value.Length, initializeWithZeros: true);

        var segmentsDiff = Math.DivRem(shiftAmount, BitsPerSegment, out shiftAmount);

        var rotation = shiftAmount == 0 ? 0 : BitsPerSegment - shiftAmount;
        var carryMask = rotation == 0 ? 0 : ~((1UL << rotation) - 1);
        var carry = 0UL;
        for (var i = result.SegmentsCount - 1; i >= segmentsDiff; i--)
        {
            var segment = value.GetSegment(i);

            result.SetSegment(i - segmentsDiff, (segment << shiftAmount) | carry);
            carry = (segment & carryMask) >> rotation;
        }

        return result;
    }

    /// <summary>
    /// Shifts a bitmask right by a given amount.
    /// </summary>
    /// <param name="value">The bitmask which is shifted right by <paramref name="shiftAmount" />.</param>
    /// <param name="shiftAmount">The amount by which <paramref name="value" /> is shifted right.</param>
    /// <returns>The result of shifting <paramref name="value" /> right by <paramref name="shiftAmount" />.</returns>
    /// <remarks>There is no difference between the arithmetic or logical right shift.</remarks>
    public static Bitmask operator >> (Bitmask value, int shiftAmount)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (shiftAmount < 0) throw new ArgumentException("The shift amount must be a non-negative number.", nameof(shiftAmount));
        
        var result = new Bitmask(value.Length, initializeWithZeros: true);

        var segmentsDiff = Math.DivRem(shiftAmount, BitsPerSegment, out shiftAmount);

        var rotation = shiftAmount == 0 ? 0 : BitsPerSegment - shiftAmount;
        var carryMask = shiftAmount == 0 ? 0 : (1UL << shiftAmount) - 1;
        var carry = 0UL;
        for (var i = 0; i < result.SegmentsCount - segmentsDiff; i++)
        {
            var segment = value.GetSegment(i);

            result.SetSegment(i + segmentsDiff, (segment >> shiftAmount) | carry);
            carry = (segment & carryMask) << rotation;
        }

        return result;
    }

    /// <summary>
    /// Shifts a bitmask right by a given amount.
    /// </summary>
    /// <param name="value">The bitmask which is shifted right by <paramref name="shiftAmount" />.</param>
    /// <param name="shiftAmount">The amount by which <paramref name="value" /> is shifted right.</param>
    /// <returns>The result of shifting <paramref name="value" /> right by <paramref name="shiftAmount" />.</returns>
    /// <remarks>There is no difference between the arithmetic or logical right shift.</remarks>
    public static Bitmask operator >>> (Bitmask value, int shiftAmount) => value >> shiftAmount;

    private static ulong BitwiseAnd(ulong a, ulong b) => a & b;
    private static ulong BitwiseOr(ulong a, ulong b) => a | b;
    private static ulong BitwiseXor(ulong a, ulong b) => a ^ b;

    private static Bitmask ExecuteBitwiseOperation(Bitmask a, Bitmask b, Func<ulong, ulong, ulong> operation)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));

        var result = new Bitmask(length: Math.Max(a.Length, b.Length), initializeWithZeros: false);
        for (var i = 0; i < result.SegmentsCount; i++)
        {
            var left = i < a.SegmentsCount ? a.GetSegment(i) : 0;
            var right = i < b.SegmentsCount ? b.GetSegment(i) : 0;

            result.SetSegment(i, operation(left, right));
        }

        return result;
    }

    private int FindLeastSignificantSetBit(bool inverse)
    {
        for (var i = this.SegmentsCount - 1; i >= 0; i--)
        {
            var currentSegment = this._segments[i];
            if (inverse)
            {
                currentSegment = ~currentSegment;
                if (i == this.SegmentsCount - 1) currentSegment = this.ApplyLastSegmentMask(currentSegment);
            }

            if (currentSegment == ZeroSegment) continue;

            return (i * BitsPerSegment + BitsPerSegment - (Bits.TrailingZeroCount(currentSegment) + 1));
        }

        return -1;
    }

    private int FindMostSignificantSetBit(bool inverse)
    {
        for (var i = 0; i < this.SegmentsCount; i++)
        {
            var currentSegment = this._segments[i];
            if (inverse)
            {
                currentSegment = ~currentSegment;
                if (i == this.SegmentsCount - 1) currentSegment = this.ApplyLastSegmentMask(currentSegment);
            }

            if (currentSegment == ZeroSegment) continue;

            return (i * BitsPerSegment + Bits.LeadingZeroCount(currentSegment));
        }

        return -1;
    }

    private void SetSegment(int index, ulong value)
    {
        this._segments[index] = value;
        if (index == this.SegmentsCount - 1) this.NormalizeLastSegment();
    }

    private ulong GetSegment(int index) => this._segments[index];

    private (int SegmentIndex, int BitIndex) Locate(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be a non-negative number.");
        if (position >= this.Length) throw new ArgumentOutOfRangeException(nameof(position), "Bit position must be less than the total number of bits.");

        var segmentIndex = Math.DivRem(position, BitsPerSegment, out var bitIndex);
        return (segmentIndex, BitsPerSegment - (bitIndex + 1));
    }

    private void NormalizeLastSegment() => this._segments[^1] = this.ApplyLastSegmentMask(this._segments[^1]);

    private ulong ApplyLastSegmentMask(ulong segment) => segment & this._lastSegmentMask;
}