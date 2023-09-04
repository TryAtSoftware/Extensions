namespace TryAtSoftware.Extensions.Collections.Tests;

using System;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class BitmaskTests
{
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
    public void SetAllShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();
            bitmask.SetAll();
            
            for (var j = 0; j < bitmask.Length; j++) Assert.True(bitmask.IsSet(j));
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
    public void UnsetAllShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();
            bitmask.UnsetAll();
            
            for (var j = 0; j < bitmask.Length; j++) Assert.False(bitmask.IsSet(j));
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
    public void BitwiseAndShouldBeExecutedSuccessfullyInPlace() => AssertCorrectInPlaceBitwiseOperation((a, b) => a.InPlaceAnd(b), (a, b) => a & b);

    [Fact]
    public void BitwiseAndShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a & b, (a, b) => a & b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseAndShouldValidateItsArguments() => AssertBitwiseOperandsAreValidatedSuccessfully((a, b) => a & b, (a, b) => a.InPlaceAnd(b));

    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b);
    
    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfullyInPlace() => AssertCorrectInPlaceBitwiseOperation((a, b) => a.InPlaceOr(b), (a, b) => a | b);

    [Fact]
    public void BitwiseOrShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a | b, (a, b) => a | b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseOrShouldValidateItsArguments() => AssertBitwiseOperandsAreValidatedSuccessfully((a, b) => a | b, (a, b) => a.InPlaceOr(b));

    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfully() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b);
    
    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfullyInPlace() => AssertCorrectInPlaceBitwiseOperation((a, b) => a.InPlaceXor(b), (a, b) => a ^ b);

    [Fact]
    public void BitwiseXorShouldBeExecutedSuccessfullyWhenLengthIsDifferent() => AssertCorrectBitwiseOperation((a, b) => a ^ b, (a, b) => a ^ b, lengthDifferenceInSegments: RandomBitmaskLength());

    [Fact]
    public void BitwiseXorShouldValidateItsArguments() => AssertBitwiseOperandsAreValidatedSuccessfully((a, b) => a ^ b, (a, b) => a.InPlaceXor(b));

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
            var result = bitmask << i;

            for (var j = 0; j < bitmask.Length - i; j++) Assert.Equal(bitmask.IsSet(i + j), result.IsSet(j));
            for (var j = 0; j < i; j++) Assert.False(result.IsSet(bitmask.Length - i + j));
        }
    }

    [Fact]
    public void LeftShiftShouldValidateItsArguments()
    {
        var bitmask = GenerateBitmask();
        
        Assert.Throws<ArgumentNullException>(() => (Bitmask)null! << 2);
        Assert.Throws<ArgumentException>(() => bitmask << -1);
    }

    [Fact]
    public void ArithmeticRightShiftShouldWorkCorrectlyWithRandomBitmask() => AssertCorrectRightShift((a, b) => a >> b);

    [Fact]
    public void LogicalRightShiftShouldWorkCorrectlyWithRandomBitmask() => AssertCorrectRightShift((a, b) => a >>> b);

    [Fact]
    public void RightShiftShouldValidateItsArguments()
    {
        var bitmask = GenerateBitmask();
        
        Assert.Throws<ArgumentNullException>(() => (Bitmask)null! >> 2);
        Assert.Throws<ArgumentNullException>(() => (Bitmask)null! >>> 2);
        Assert.Throws<ArgumentException>(() => bitmask >> -1);
        Assert.Throws<ArgumentException>(() => bitmask >>> -1);
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
    public void FindMostSignificantSetBitShouldWorkCorrectlyWithBitmaskOfZeros()
    {
        var bitmask = InstantiateBitmask();
        Assert.Equal(-1, bitmask.FindMostSignificantSetBit());
    }

    [Fact]
    public void FindMostSignificantSetBitShouldWorkCorrectlyInGeneral()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);
        for (var i = 0; i < bitmask.Length; i++)
        {
            var result = bitmask.FindMostSignificantSetBit();
            Assert.Equal(i, result);

            bitmask.Unset(i);
        }
    }

    [Fact]
    public void FindMostSignificantSetBitShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var result = bitmask.FindMostSignificantSetBit();
            Assert.True(bitmask.IsSet(result));

            for (var j = 0; j < result; j++) Assert.False(bitmask.IsSet(j));
        }
    }

    [Fact]
    public void FindMostSignificantUnsetBitShouldWorkCorrectlyWithBitmaskOfOnes()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: false);
        Assert.Equal(-1, bitmask.FindMostSignificantUnsetBit());
    }

    [Fact]
    public void FindMostSignificantUnsetBitShouldWorkCorrectlyInGeneral()
    {
        var bitmask = InstantiateBitmask(initializeWithZeros: true);
        for (var i = 0; i < bitmask.Length; i++)
        {
            var result = bitmask.FindMostSignificantUnsetBit();
            Assert.Equal(i, result);

            bitmask.Set(i);
        }
    }

    [Fact]
    public void FindMostSignificantUnsetBitShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var result = bitmask.FindMostSignificantUnsetBit();
            Assert.False(bitmask.IsSet(result));

            for (var j = 0; j < result; j++) Assert.True(bitmask.IsSet(j));
        }
    }

    [Fact]
    public void CountSetBitsShouldWorkCorrectlyWithHomogenousBitmasks()
    {
        var zeroBitmask = InstantiateBitmask(initializeWithZeros: true);
        Assert.Equal(0, zeroBitmask.CountSetBits());

        var oneBitmask = InstantiateBitmask(initializeWithZeros: false);
        Assert.Equal(oneBitmask.Length, oneBitmask.CountSetBits());
    }

    [Fact]
    public void CountSetBitsShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var expected = 0;
            for (var j = 0; j < bitmask.Length; j++)
                if (bitmask.IsSet(j)) expected++;
            
            Assert.Equal(expected, bitmask.CountSetBits());
        }
    }

    [Fact]
    public void CountUnsetBitsShouldWorkCorrectlyWithHomogenousBitmasks()
    {
        var zeroBitmask = InstantiateBitmask(initializeWithZeros: true);
        Assert.Equal(zeroBitmask.Length, zeroBitmask.CountUnsetBits());

        var oneBitmask = InstantiateBitmask(initializeWithZeros: false);
        Assert.Equal(0, oneBitmask.CountUnsetBits());
    }

    [Fact]
    public void CountUnsetBitsShouldWorkCorrectlyWithRandomBitmask()
    {
        for (var i = 0; i < 100; i++)
        {
            var bitmask = GenerateBitmask();

            var expected = 0;
            for (var j = 0; j < bitmask.Length; j++)
                if (!bitmask.IsSet(j)) expected++;
            
            Assert.Equal(expected, bitmask.CountUnsetBits());
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

    private static void AssertCorrectInPlaceBitwiseOperation(Action<Bitmask, Bitmask> executeOperationInPlace, Func<bool, bool, bool> getExpectedBit)
    {
        var length = RandomBitmaskLength();

        var originalBitmask = GenerateBitmask(length);
        var otherBitmask = GenerateBitmask(length);

        var expected = new Bitmask(length, initializeWithZeros: true);
        var otherBitmaskCopy = new Bitmask(length, initializeWithZeros: true);
        for (var i = 0; i < length; i++)
        {
            var expectedBit = getExpectedBit(originalBitmask.IsSet(i), otherBitmask.IsSet(i));
            if (expectedBit) expected.Set(i);

            if (otherBitmask.IsSet(i)) otherBitmaskCopy.Set(i);
        }

        executeOperationInPlace(originalBitmask, otherBitmask);

        for (var i = 0; i < length; i++)
        {
            Assert.Equal(expected.IsSet(i), originalBitmask.IsSet(i));
            Assert.Equal(otherBitmaskCopy.IsSet(i), otherBitmask.IsSet(i)); // Assert the `other` bitmask instance remains unchanged.
        }
    }

    private static void AssertBitwiseOperandsAreValidatedSuccessfully(Func<Bitmask, Bitmask, Bitmask> executeOperation, Action<Bitmask, Bitmask> executeOperationInPlace)
    {
        var bitmask = GenerateBitmask();
        Assert.Throws<ArgumentNullException>(() => executeOperation(bitmask, null!));
        Assert.Throws<ArgumentNullException>(() => executeOperation(null!, bitmask));

        Assert.Throws<ArgumentNullException>(() => executeOperationInPlace(bitmask, null!));

        var bitmaskWithDifferentLength = new Bitmask(bitmask.Length + RandomizationHelper.RandomInteger(1, 10, upperBoundIsExclusive: false), initializeWithZeros: true);
        Assert.Throws<InvalidOperationException>(() => executeOperationInPlace(bitmask, bitmaskWithDifferentLength));
    }

    private static void AssertCorrectRightShift(Func<Bitmask, int, Bitmask> compute)
    {
        var bitmask = GenerateBitmask();

        for (var i = 0; i <= bitmask.Length; i++)
        {
            var result = compute(bitmask, i);

            for (var j = 0; j < bitmask.Length - i; j++) Assert.Equal(bitmask.IsSet(j), result.IsSet(i + j));
            for (var j = 0; j < i; j++) Assert.False(result.IsSet(j));
        }
    }

    private static int RandomBitmaskLength() => RandomizationHelper.RandomInteger(100, 1000);
}