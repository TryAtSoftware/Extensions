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
        
        Assert.Equal(Math.Ceiling(randomCount / 64.0), bitmask.SegmentsCount);
        Assert.Equal(randomCount, bitmask.Count);
        Assert.True(bitmask.IsZero);
        Assert.False(bitmask.IsOne);
        for (var i = 0; i < randomCount; i++) Assert.False(bitmask.IsSet(i));
    }

    [Fact]
    public void BitmaskShouldBeInitializedSuccessfullyWithOnes()
    {
        var randomCount = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(randomCount, initializeWithZeros: false);
        
        Assert.Equal(Math.Ceiling(randomCount / 64.0), bitmask.SegmentsCount);
        Assert.Equal(randomCount, bitmask.Count);
        Assert.False(bitmask.IsZero);
        Assert.True(bitmask.IsOne);
        for (var i = 0; i < randomCount; i++) Assert.True(bitmask.IsSet(i));
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
}