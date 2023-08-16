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

    [Fact]
    public void BitmaskShouldBeInitializedSuccessfullyWithZeros()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: true);
        for (var i = 0; i < bitmask.Length; i++) Assert.False(bitmask.IsSet(i));
    }
    
    [Fact]
    public void BitmaskShouldBeInitializedSuccessfullyWithOnes()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);
        for (var i = 0; i < bitmask.Length; i++) Assert.True(bitmask.IsSet(i));
    }

    [Fact]
    public void BitwiseAndShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b);
    
    [Fact]
    public void BitwiseAndShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    
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
    public void BitwiseOrShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    
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
    public void BitwiseXorShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b, lengthDifferenceInSegments: RandomizationHelper.RandomInteger(2, 10));
    
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

        for (var i = 0; i < bitmask.Length; i++) Assert.NotEqual(bitmask.IsSet(i), result.IsSet(i));
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

    private static Bitmask InstantiateBitmask(bool initializeWithZeros = true) => InstantiateBitmask(RandomBitmaskLength(), initializeWithZeros);

    private static Bitmask InstantiateBitmask(int length, bool initializeWithZeros = true)
    {
        var bitmask = new Bitmask(length, initializeWithZeros);

        Assert.Equal(length, bitmask.Length);
        return bitmask;
    }

    private static (bool[] Bits, Bitmask Bitmask) GenerateBitmask(int? length = null)
    {
        var bitsCount = length ?? RandomBitmaskLength();
        var bitmask = new Bitmask(bitsCount, initializeWithZeros: true);
        var randomBits = new bool[bitsCount];
    
        for (var i = 0; i < bitsCount; i++)
        {
            randomBits[i] = RandomizationHelper.RandomProbability();
            if (randomBits[i]) bitmask.Set(i);
        }
    
        return (randomBits, bitmask);
    }
    
    private static void AssertCorrectBitwiseOperation(Func<Bitmask, Bitmask, Bitmask> compute, Func<bool, bool, bool> getExpectedBit, int lengthDifferenceInSegments = 0)
    {
        var baseBitmaskLength = RandomBitmaskLength();
        var totalBitmaskLength = baseBitmaskLength + lengthDifferenceInSegments;
    
        var (bits1, bitmask1) = GenerateBitmask(baseBitmaskLength);
        var (bits2, bitmask2) = GenerateBitmask(totalBitmaskLength);
    
        var expected = new Bitmask(totalBitmaskLength, initializeWithZeros: true);
        for (var i = 0; i < totalBitmaskLength; i++)
        {
            var expectedBit = getExpectedBit(i < baseBitmaskLength && bits1[i], bits2[i]);
            if (expectedBit) expected.Set(i);
        }
    
        var results = new[] { compute(bitmask1, bitmask2), compute(bitmask2, bitmask1) };
        foreach (var result in results)
        {
            Assert.NotNull(result);
            Assert.Equal(totalBitmaskLength, result.Length);
            
            for (var i = 0; i < totalBitmaskLength; i++)
                Assert.Equal(expected.IsSet(i), result.IsSet(i));
        }
    }

    private static int RandomBitmaskLength() => RandomizationHelper.RandomInteger(100, 1000);
}