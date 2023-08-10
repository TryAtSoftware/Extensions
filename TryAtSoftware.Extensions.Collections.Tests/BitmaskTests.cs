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

        for (var i = 0; i < bitmask.SegmentsCount; i++) Assert.Equal(0, bitmask.GetSegment(i));
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

        for (var i = 0; i < bitmask.SegmentsCount; i++) Assert.Equal(~0, bitmask.GetSegment(i));
    }

    [Fact]
    public void SegmentShouldBeModifiedSuccessfully()
    {
        var segmentsCount = RandomizationHelper.RandomInteger(10, 100);
        var randomSegments = new long[segmentsCount];
        var bitmask = new Bitmask(segmentsCount * Bitmask.BitsPerSegment, initializeWithZeros: true);

        for (var i = 0; i < segmentsCount; i++)
        {
            // This is a temporary solution until TryAtSoftware.Randomizer exposes methods to randomize numbers of various types.
            for (var j = 0; j < 4; j++)
            {
                randomSegments[i] = (randomSegments[i] << 16);
                randomSegments[i] |= (uint)RandomizationHelper.RandomInteger(0, short.MaxValue + 1);
            }
            
            bitmask.SetSegment(i, randomSegments[i]);
        }
        
        for (var i = 0; i < segmentsCount; i++) Assert.Equal(randomSegments[i], bitmask.GetSegment(i));
    }

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

    private static void AssertDimensions(Bitmask bitmask, int randomCount)
    {
        Assert.Equal(Math.Ceiling(randomCount / (double) Bitmask.BitsPerSegment), bitmask.SegmentsCount);
        Assert.Equal(randomCount, bitmask.Count);
    }
}