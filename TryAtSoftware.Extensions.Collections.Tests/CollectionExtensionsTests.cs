namespace TryAtSoftware.Extensions.Collections.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class CollectionExtensionsTests
{
    [Fact]
    public void OrEmptyIfNullShouldReturnTheSameCollectionIfItIsNotNull()
    {
        var collection = GetStandardCollection();
        var result = collection.OrEmptyIfNull();
        Assert.NotNull(result);
        Assert.Same(collection, result);
    }

    [Fact]
    public void OrEmptyIfNullShouldReturnEmptyCollectionIfNullIsPassed()
    {
        var result = ((IEnumerable<object>)null).OrEmptyIfNull();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void IgnoreNullValuesShouldThrowIfThePassedCollectionIsNull() => Assert.Throws<ArgumentNullException>(() => ((IEnumerable<object>)null).IgnoreNullValues());

    [Fact]
    public void IgnoreNullValuesShouldReturnCollectionWithNonNullableElements()
    {
        var dirtyCollection = new object[]
        {
            null, new(), new(), null, new(),
            null
        };
        var cleanCollection = dirtyCollection.IgnoreNullValues().ToList();
        Assert.NotNull(cleanCollection);
        Assert.Equal(3, cleanCollection.Count);

        foreach (var element in cleanCollection) Assert.NotNull(element);
    }

    [Fact]
    public void IgnoreDefaultValuesShouldThrowIfThePassedCollectionIsNull() => Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).IgnoreDefaultValues());

    [Fact]
    public void IgnoreDefaultValuesShouldReturnCollectionWithElementsNotEqualToDefault()
    {
        var dirtyCollection = new[]
        {
            0, 1, 2, 0, 3,
            4, 0, 0, 5, 0
        };
        var cleanCollection = dirtyCollection.IgnoreDefaultValues().ToList();
        Assert.NotNull(cleanCollection);
        Assert.Equal(5, cleanCollection.Count);

        foreach (var element in cleanCollection) Assert.NotEqual(default, element);
    }

    [Theory]
    [MemberData(nameof(GetConcatenateWithTestData))]
    public void ConcatenateWithShouldWorkCorrectly(IEnumerable<int> a, IEnumerable<int> b, int[] expected)
    {
        var concatenationResult = a.ConcatenateWith(b);
        Assert.NotNull(concatenationResult);

        var iteratedResult = concatenationResult.ToArray();
        Assert.Equal(expected.Length, iteratedResult.Length);
        foreach (var (expectedElement, actualElement) in expected.Zip(iteratedResult)) Assert.Equal(expectedElement, actualElement);
    }

    [Fact]
    public void SafeWhereShouldThrowIfThePassedCollectionIsNull() => Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).SafeWhere(IsOdd));

    [Fact]
    public void SafeWhereShouldReturnTheSameCollectionIfNoConditionIsPassed()
    {
        var collection = GetStandardCollection();
        var result = collection.SafeWhere(null);
        Assert.NotNull(result);
        Assert.Same(collection, result);
    }

    [Fact]
    public void SafeWhereShouldRespectTheProvidedCondition()
    {
        var collection = Repeat(GetStandardCollection(), 5);
        var oddNumbersMap = GetElementsMap(collection.Where(IsOdd));

        var result = collection.SafeWhere(IsOdd);
        var resultMap = GetElementsMap(result);

        Assert.Equal(oddNumbersMap, resultMap);
    }

    public static IEnumerable<object[]> GetConcatenateWithTestData()
    {
        yield return new object[] { null, null, Array.Empty<object>() };

        var standardCollection = GetStandardCollection().ToArray();
        yield return new object[] { null, standardCollection, standardCollection };
        yield return new object[] { standardCollection, null, standardCollection };

        const int elementsCount = 5;
        var oddNumbers = new int[elementsCount];
        var evenNumbers = new int[elementsCount];
        for (var i = 0; i < elementsCount; i++)
        {
            evenNumbers[i] = i * 2;
            oddNumbers[i] = i * 2 + 1;
        }

        yield return new object[] { evenNumbers, oddNumbers, ConcatenateTwoArrays(evenNumbers, oddNumbers) };
        yield return new object[] { oddNumbers, evenNumbers, ConcatenateTwoArrays(oddNumbers, evenNumbers) };
    }

    private static T[] ConcatenateTwoArrays<T>(T[] a, T[] b)
    {
        Assert.NotNull(a);
        Assert.NotNull(b);

        var result = new T[a.Length + b.Length];
        Array.Copy(a, result, a.Length);
        Array.Copy(b, 0, result, a.Length, b.Length);
        return result;
    }

    private static bool IsOdd(int a) => a % 2 == 0;

    private static IEnumerable<int> GetStandardCollection() => Enumerable.Range(0, 5);

    private static IEnumerable<T> Repeat<T>(IEnumerable<T> collection, int n)
    {
        foreach (var el in collection)
        {
            for (int i = 0; i < n; i++) yield return el;
        }
    }
        
    private static IDictionary<T, int> GetElementsMap<T>(IEnumerable<T> collection)
    {
        Assert.NotNull(collection);

        var map = new Dictionary<T, int>();
        foreach (var el in collection)
        {
            if (!map.ContainsKey(el)) map[el] = 0;
            map[el]++;
        }

        return map;
    }
}