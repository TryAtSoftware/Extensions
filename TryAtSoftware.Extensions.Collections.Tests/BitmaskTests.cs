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
    public void BitwiseAndShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseAndShouldValidateItsArguments()
    {
        var bitmask = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask & null!);
        Assert.Throws<ArgumentNullException>(() => null! & bitmask);
    }

    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b);

    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseOrShouldValidateItsArguments()
    {
        var bitmask = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask | null!);
        Assert.Throws<ArgumentNullException>(() => null! | bitmask);
    }

    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b);

    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseXorShouldValidateItsArguments()
    {
        var bitmask = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => bitmask ^ null!);
        Assert.Throws<ArgumentNullException>(() => null! ^ bitmask);
    }

    [Fact]
    public void BitwiseNotShouldBeExecutedSuccessfully()
    {
        var bitmask = GenerateBitmask();
        var result = ~bitmask;

        for (var i = 0; i < bitmask.Length; i++) Assert.NotEqual(bitmask.IsSet(i), result.IsSet(i));
    }

    [Fact]
    public void BitwiseNotShouldValidateItsArguments() => Assert.Throws<ArgumentNullException>(() => ~(Bitmask)null!);

    [Fact]
    public void LeftShiftShouldWorkCorrectlyWithRandomBitmask()
    {
        var bitmask = GenerateBitmask();

        for (var i = 0; i <= bitmask.Length; i++)
        {
            this._testOutputHelper.WriteLine($"Rotating {i} positions to the left.");
            var result = bitmask << i;

            for (var j = 0; j < bitmask.Length - i; j++) Assert.Equal(bitmask.IsSet(i + j), result.IsSet(j));
            for (var j = 0; j < i; j++) Assert.False(result.IsSet(bitmask.Length - i + j));
        }
        
    }

    [Fact]
    public void BitPositionShouldBeValidated()
    {
        var bitmask = InstantiateBitmask();

        var negativeIndex = -1 * RandomizationHelper.RandomInteger(1, 100);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(negativeIndex));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Set(negativeIndex));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Unset(negativeIndex));

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(bitmask.Length));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Set(bitmask.Length));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Unset(bitmask.Length));

        var largeIndex = bitmask.Length + RandomizationHelper.RandomInteger(1, 100);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.IsSet(largeIndex));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Set(largeIndex));
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmask.Unset(largeIndex));
    }

    [Fact]
    public void FindLeastSignificantSetBitShouldWorkCorrectlyWithBitmaskOfZeros()
    {
        var bitmask = InstantiateBitmask();
        Assert.Equal(-1, bitmask.FindLeastSignificantSetBit());
    }

    [Fact]
    public void FindLeastSignificantSetBitShouldWorkCorrectlyInGeneral()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);
        for (var i = bitmask.Length - 1; i >= 0; i--)
        {
            var result = bitmask.FindLeastSignificantSetBit();
            Assert.Equal(i, result);

            bitmask.Unset(i);
        }
    }

    [Fact]
    public void FindLeastSignificantSetBitShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var result = bitmask.FindLeastSignificantSetBit();
            Assert.True(bitmask.IsSet(result));

            for (var j = result + 1; j < bitmask.Length; j++) Assert.False(bitmask.IsSet(j));
        }
    }

    [Fact]
    public void FindLeastSignificantUnsetBitShouldWorkCorrectlyWithBitmaskOfOnes()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);
        Assert.Equal(-1, bitmask.FindLeastSignificantUnsetBit());
    }

    [Fact]
    public void FindLeastSignificantUnsetBitShouldWorkCorrectlyInGeneral()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: true);
        for (var i = bitmask.Length - 1; i >= 0; i--)
        {
            var result = bitmask.FindLeastSignificantUnsetBit();
            Assert.Equal(i, result);

            bitmask.Set(i);
        }
    }

    [Fact]
    public void FindLeastSignificantUnsetBitShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var result = bitmask.FindLeastSignificantUnsetBit();
            Assert.False(bitmask.IsSet(result));

            for (var j = result + 1; j < bitmask.Length; j++) Assert.True(bitmask.IsSet(j));
        }
    }

    [Fact]
    public void ToStringShouldReturnCorrectBitmaskRepresentation()
    {
        var bitmask = GenerateBitmask();
        var stringRepresentation = bitmask.ToString();
        Assert.Equal(bitmask.Length, stringRepresentation.Length);

        for (var i = 0; i < bitmask.Length; i++) Assert.Equal(bitmask.IsSet(i) ? '1' : '0', stringRepresentation[i]);
    }

    private static Bitmask InstantiateBitmask(bool initializeWithZeros = true) => InstantiateBitmask(RandomBitmaskLength(), initializeWithZeros);

    private static Bitmask InstantiateBitmask(int length, bool initializeWithZeros = true)
    {
        var bitmask = new Bitmask(length, initializeWithZeros);

        Assert.Equal(length, bitmask.Length);
        return bitmask;
    }

    private static Bitmask GenerateBitmask(int? length = null)
    {
        var bitsCount = length ?? RandomBitmaskLength();
        var bitmask = new Bitmask(bitsCount, initializeWithZeros: true);

        for (var i = 0; i < bitsCount; i++)
            if (RandomizationHelper.RandomProbability())
                bitmask.Set(i);

        return bitmask;
    }

    private static void AssertCorrectBitwiseOperation(Func<Bitmask, Bitmask, Bitmask> compute, Func<bool, bool, bool> getExpectedBit, int lengthDifferenceInSegments = 0)
    {
        var baseBitmaskLength = RandomBitmaskLength();
        var totalBitmaskLength = baseBitmaskLength + lengthDifferenceInSegments;

        var bitmask1 = GenerateBitmask(baseBitmaskLength);
        var bitmask2 = GenerateBitmask(totalBitmaskLength);

        var expected = new Bitmask(totalBitmaskLength, initializeWithZeros: true);
        for (var i = 0; i < totalBitmaskLength; i++)
        {
            var expectedBit = getExpectedBit(i < baseBitmaskLength && bitmask1.IsSet(i), bitmask2.IsSet(i));
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