namespace TryAtSoftware.Extensions.Collections.Tests;

using System;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;
using Xunit.Abstractions;

public class BitmaskTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BitmaskTests(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }

    [Fact]
    public void BitsShouldBeIndexedSuccessfully()
    {
        var bitmask = InstantiateBitmask();

        for (var i = 0; i < 100; i++)
        {
            var randomIndex = RandomizationHelper.RandomInteger(0, bitmask.Length);
            bitmask.Set(randomIndex);
            for (var j = 0; j < bitmask.Length; j++) Assert.Equal(j == randomIndex, bitmask.IsSet(j));

            bitmask.Unset(randomIndex);
            for (var j = 0; j < bitmask.Length; j++) Assert.False(bitmask.IsSet(j));
        }
    }

    [Fact]
    public void MultiSetShouldWorkCorrectly()
    {
        var bitmask = InstantiateBitmask();

        var randomIndex = RandomizationHelper.RandomInteger(0, bitmask.Length);
        for (var i = 0; i < 10; i++)
        {
            bitmask.Set(randomIndex);
            for (var j = 0; j < bitmask.Length; j++) Assert.Equal(j == randomIndex, bitmask.IsSet(j));
        }
    }

    [Fact]
    public void MultiUnsetShouldWorkCorrectly()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);

        var randomIndex = RandomizationHelper.RandomInteger(0, bitmask.Length);
        for (var i = 0; i < 10; i++)
        {
            bitmask.Unset(randomIndex);
            for (var j = 0; j < bitmask.Length; j++) Assert.Equal(j != randomIndex, bitmask.IsSet(j));
        }
    }

    // [Fact]
    // public void BitmaskShouldBeInitializedSuccessfullyWithZeros()
    // {
    //     var randomCount = RandomizationHelper.RandomInteger(100, 1000);
    //     var bitmask = new Bitmask(randomCount, initializeWithZeros: true);
    //
    //     AssertDimensions(bitmask, randomCount);
    //     for (var i = 0; i < randomCount; i++) Assert.False(bitmask.IsSet(i));
    //
    //     for (var i = 0; i < bitmask.SegmentsCount; i++) Assert.Equal(0UL, bitmask.GetSegment(i));
    // }
    //
    // [Fact]
    // public void BitmaskShouldBeInitializedSuccessfullyWithOnes()
    // {
    //     // var randomCount = RandomizationHelper.RandomInteger(100, 1000);
    //     var randomCount = 553;
    //     this._testOutputHelper.WriteLine($"Initializing bitmask with {randomCount} bits");
    //     var bitmask = new Bitmask(randomCount, initializeWithZeros: false);
    //
    //     AssertDimensions(bitmask, randomCount);
    //     for (var i = 0; i < randomCount; i++) Assert.True(bitmask.IsSet(i));
    // }
    //
    // [Fact]
    // public void SegmentShouldBeModifiedSuccessfully()
    // {
    //     var (segments, bitmask) = GenerateBitmask();
    //     for (var i = 0; i < segments.Length; i++) Assert.Equal(segments[i], bitmask.GetSegment(i));
    // }
    //
    // [Fact]
    // public void BitwiseAndShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b);
    //
    // [Fact]
    // public void BitwiseAndShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    //
    // [Fact]
    // public void BitwiseAndShouldValidateItsArguments()
    // {
    //     var (_, bitmask) = GenerateBitmask();
    //     Assert.Throws<ArgumentNullException>(() => bitmask & null!);
    //     Assert.Throws<ArgumentNullException>(() => null! & bitmask);
    // }
    //
    // [Fact]
    // public void BitwiseOrShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b);
    //
    // [Fact]
    // public void BitwiseOrShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    //
    // [Fact]
    // public void BitwiseOrShouldValidateItsArguments()
    // {
    //     var (_, bitmask) = GenerateBitmask();
    //     Assert.Throws<ArgumentNullException>(() => bitmask | null!);
    //     Assert.Throws<ArgumentNullException>(() => null! | bitmask);
    // }
    //
    // [Fact]
    // public void BitwiseXorShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b);
    //
    // [Fact]
    // public void BitwiseXorShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    //
    // [Fact]
    // public void BitwiseXorShouldValidateItsArguments()
    // {
    //     var (_, bitmask) = GenerateBitmask();
    //     Assert.Throws<ArgumentNullException>(() => bitmask ^ null!);
    //     Assert.Throws<ArgumentNullException>(() => null! ^ bitmask);
    // }
    //
    // [Fact]
    // public void BitwiseNotShouldBeExecutedSuccessfully()
    // {
    //     var (_, bitmask) = GenerateBitmask();
    //     var result = ~bitmask;
    //     
    //     this._testOutputHelper.WriteLine(bitmask.ToString());
    //     this._testOutputHelper.WriteLine(result.ToString());
    //
    //     for (var i = 0; i < bitmask.Length; i++) Assert.NotEqual(bitmask.IsSet(i), result.IsSet(i));
    // }
    //
    // [Fact]
    // public void BitwiseNotShouldValidateItsArguments() => Assert.Throws<ArgumentNullException>(() => ~(Bitmask)null!);
    //
    // [Fact]
    // public void BitPositionShouldBeValidated()
    // {
    //     var randomCount = RandomizationHelper.RandomInteger(100, 1000);
    //     var bitmask = new Bitmask(randomCount, initializeWithZeros: true);
    //
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(-1 * RandomizationHelper.RandomInteger(1, 100)));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(randomCount));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(randomCount + RandomizationHelper.RandomInteger(1, 100)));
    // }
    //
    // [Fact]
    // public void GetSegmentShouldValidateIndex()
    // {
    //     var randomCount = RandomizationHelper.RandomInteger(100, 1000);
    //     var bitmask = new Bitmask(randomCount, initializeWithZeros: true);
    //
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(-1 * RandomizationHelper.RandomInteger(1, 100)));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(bitmask.SegmentsCount));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.GetSegment(bitmask.SegmentsCount + RandomizationHelper.RandomInteger(1, 100)));
    // }
    //
    // [Fact]
    // public void SetSegmentShouldValidateIndex()
    // {
    //     var randomCount = RandomizationHelper.RandomInteger(100, 1000);
    //     var bitmask = new Bitmask(randomCount, initializeWithZeros: true);
    //
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(-1 * RandomizationHelper.RandomInteger(1, 100), 0));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(bitmask.SegmentsCount, 0));
    //     Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.SetSegment(bitmask.SegmentsCount + RandomizationHelper.RandomInteger(1, 100), 0));
    // }
    //

    private static Bitmask InstantiateBitmask(bool initializeWithZeros = true)
    {
        var length = RandomizationHelper.RandomInteger(100, 1000);
        var bitmask = new Bitmask(length, initializeWithZeros);

        Assert.Equal(length, bitmask.Length);
        return bitmask;
    }

    // private static (ulong[] Segments, Bitmask Bitmask) GenerateBitmask(int? length = null)
    // {
    //     var bitsCount = length ?? RandomizationHelper.RandomInteger(10, 1000);
    //     var bitmask = new Bitmask(bitsCount, initializeWithZeros: true);
    //     var randomSegments = new ulong[bitmask.SegmentsCount];
    //
    //     for (var i = 0; i < bitmask.SegmentsCount; i++)
    //     {
    //         randomSegments[i] = RandomizationHelper.RandomUnsignedLongInteger();
    //         bitmask.SetSegment(i, randomSegments[i]);
    //     }
    //
    //     return (randomSegments, bitmask);
    // }
    //
    // private static void AssertCorrectBitwiseOperation(Func<Bitmask, Bitmask, Bitmask> compute, Func<ulong, ulong, ulong> getExpectedSegment, int lengthDifferenceInSegments = 0)
    // {
    //     var baseSegmentsLength = RandomizationHelper.RandomInteger(10, 100);
    //     var baseBitmaskLength = baseSegmentsLength * Bitmask.BitsPerSegment;
    //     var totalBitmaskLength = baseBitmaskLength + lengthDifferenceInSegments * Bitmask.BitsPerSegment;
    //
    //     var (segments1, bitmask1) = GenerateBitmask(baseBitmaskLength);
    //     var (segments2, bitmask2) = GenerateBitmask(totalBitmaskLength);
    //
    //     var expected = new Bitmask(totalBitmaskLength, initializeWithZeros: false);
    //     for (var i = 0; i < baseSegmentsLength; i++) expected.SetSegment(i, getExpectedSegment(segments1[i], segments2[i]));
    //     for (var i = 0; i < lengthDifferenceInSegments; i++) expected.SetSegment(baseSegmentsLength + i, getExpectedSegment(0, segments2[baseSegmentsLength + i]));
    //
    //     var results = new[] { compute(bitmask1, bitmask2), compute(bitmask2, bitmask1) };
    //     foreach (var result in results)
    //     {
    //         Assert.NotNull(result);
    //         Assert.Equal(totalBitmaskLength, result.Length);
    //         
    //         for (var i = 0; i < totalBitmaskLength; i++)
    //             Assert.Equal(expected.IsSet(i), result.IsSet(i));
    //     }
    // }
    //
    // private static void AssertDimensions(Bitmask bitmask, int randomCount)
    // {
    //     Assert.Equal(Math.Ceiling(randomCount / (double)Bitmask.BitsPerSegment), bitmask.SegmentsCount);
    //     Assert.Equal(randomCount, bitmask.Length);
    // }
}