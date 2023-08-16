namespace TryAtSoftware.Extensions.Collections.Tests;

using System;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class BitmaskTests
{
    [Fact]
    public void BitmaskShouldBeInitializedSuccessfullyWithZeros()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: true);

        AssertDimensions(bitmask, randomCount);
        Assert.True(bitmask.IsZero);
        Assert.False(bitmask.IsOne);
        for (var i = 0; i < randomCount; i++) Assert.False(bitmask.IsSet(i));

        for (var i = 0; i < bitmask.SegmentsCount; i++) Assert.Equal(0UL, bitmask.GetSegment(i));
    }

    [Fact]
    public void BitmaskShouldBeInitializedSuccessfullyWithOnes()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: false);

        AssertDimensions(bitmask, randomCount);
        Assert.False(bitmask.IsZero);
        Assert.True(bitmask.IsOne);
        for (var i = 0; i < randomCount; i++) Assert.True(bitmask.IsSet(i));

        for (var i = 0; i < bitmask.SegmentsCount; i++) Assert.Equal(~0UL, bitmask.GetSegment(i));
    }

    [Fact]
    public void SegmentShouldBeModifiedSuccessfully()
    {
        var (segments, bitmask) = GenerateBitmask();
        for (var i = 0; i < segments.Length; i++) Assert.Equal(segments[i], bitmask.GetSegment(i));
    }

    [Fact]
    public void BitwiseAndShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b);

    [Fact]
    public void BitwiseAndShouldValidateItsArguments()
    {
        var (_, bitmask) = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask & null!);
        Assert.Throws<ArgumentNullException>(() => null! & bitmask);
    }
    
    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b);

    [Fact]
    public void BitwiseOrShouldValidateItsArguments()
    {
        var (_, bitmask) = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask | null!);
        Assert.Throws<ArgumentNullException>(() => null! | bitmask);
    }
    
    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b);

    [Fact]
    public void BitwiseXorShouldValidateItsArguments()
    {
        var (_, bitmask) = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask ^ null!);
        Assert.Throws<ArgumentNullException>(() => null! ^ bitmask);
    }

    [Fact]
    public void BitwiseNotShouldBeExecutedSuccessfully()
    {
        var (_, bitmask) = GenerateBitmask();
        var result = ~bitmask;

        for (var i = 0; i < bitmask.Count; i++) Assert.NotEqual(bitmask.IsSet(i), result.IsSet(i));
    }

    [Fact]
    public void BitwiseNotShouldValidateItsArguments() => Assert.Throws<ArgumentNullException>(() => ~(Bitmask)null!);

    [Fact]
    public void BitPositionShouldBeValidated()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: true);

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(-1 * RandomizationHelper.RandomInteger(1, 100)));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(randomCount));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(randomCount + RandomizationHelper.RandomInteger(1, 100)));
    }

    [Fact]
    public void GetSegmentShouldValidateIndex()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: true);

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(-1 * RandomizationHelper.RandomInteger(1, 100)));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(bitmask.SegmentsCount));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(bitmask.SegmentsCount + RandomizationHelper.RandomInteger(1, 100)));
    }

    [Fact]
    public void SetSegmentShouldValidateIndex()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: true);

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(-1 * RandomizationHelper.RandomInteger(1, 100), 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(bitmask.SegmentsCount, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(bitmask.SegmentsCount + RandomizationHelper.RandomInteger(1, 100), 0));
    }

    private static (ulong[] Segments, Bitmask Bitmask) GenerateBitmask(int? length = null)
    {
        var bitsCount = length ?? RandomizationHelper.RandomInteger(10, 100);
        var bitmask = new Bitmask(bitsCount * Bitmask.BitsPerSegment, initializeWithZeros: true);
        var randomSegments = new ulong[bitmask.SegmentsCount];

        for (var i = 0; i < bitmask.SegmentsCount; i++)
        {
            randomSegments[i] = RandomizationHelper.RandomUnsignedLongInteger();
            bitmask.SetSegment(i, randomSegments[i]);
        }

        return (randomSegments, bitmask);
    }

    private static void AssertCorrectBitwiseOperation(Func<Bitmask, Bitmask, Bitmask> compute, Func<ulong, ulong, ulong> getExpectedSegment)
    {
        var segmentsLength = RandomizationHelper.RandomInteger(10, 100);
        var bitmaskLength = segmentsLength * Bitmask.BitsPerSegment - RandomizationHelper.RandomInteger(0, Bitmask.BitsPerSegment);

        var (segments1, bitmask1) = GenerateBitmask(bitmaskLength);
        var (segments2, bitmask2) = GenerateBitmask(bitmaskLength);
        var result = compute(bitmask1, bitmask2);

        var expected = new Bitmask(bitmaskLength, initializeWithZeros: false);
        for (var i = 0; i < segmentsLength; i++)
        {
            var expectedSegment = getExpectedSegment(segments1[i], segments2[i]);
            expected.SetSegment(i, expectedSegment);
        }

        for (var i = 0; i < bitmaskLength; i++) Assert.Equal(expected.IsSet(i), result.IsSet(i));
    }

    private static void AssertDimensions(Bitmask bitmask, int randomCount)
    {
        Assert.Equal(Math.Ceiling(randomCount / (double)Bitmask.BitsPerSegment), bitmask.SegmentsCount);
        Assert.Equal(randomCount, bitmask.Count);
    }
}