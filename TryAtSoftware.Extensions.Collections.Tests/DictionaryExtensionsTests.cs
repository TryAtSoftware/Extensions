namespace TryAtSoftware.Extensions.Collections.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class DictionaryExtensionsTests
{
    [Fact]
    public void OrEmptyIfNullShouldReturnTheSameDictionaryIfItIsNotNull()
    {
        var standardCollection = TestsHelper.GetStandardCollection();
        var dictionary = TestsHelper.GetElementsMap(standardCollection);

        var result = dictionary.OrEmptyIfNull();
        Assert.NotNull(result);
        Assert.Same(dictionary, result);
    }

    [Fact]
    public void OrEmptyIfNullShouldReturnEmptyDictionaryIfNullIsPassed()
    {
        var result = ((IDictionary<object, object>?)null).OrEmptyIfNull();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void EnsureValueShouldThrowIfTheExtendedDictionaryIsNull()
        => Assert.Throws<ArgumentNullException>(() => ((IDictionary<object, object>)null!).EnsureValue(1));

    [Fact]
    public void EnsureValueShouldHandleExistingValues()
    {
        var dictionary = new Dictionary<int, List<int>>();
        var key = RandomizationHelper.RandomInteger(1, 100);
        var value = new List<int>();
        dictionary[key] = value;

        var ensuredValue = dictionary.EnsureValue(key);
        Assert.Same(value, ensuredValue);
    }

    [Fact]
    public void EnsureValueShouldHandleNonExistingValues()
    {
        var dictionary = new Dictionary<int, List<int>>();
        var key = RandomizationHelper.RandomInteger(1, 100);

        var ensuredValue = dictionary.EnsureValue(key);
        Assert.NotNull(ensuredValue);
        Assert.Empty(ensuredValue);
    }

    [Fact]
    public void AsReadOnlyDictionaryShouldHandleNull()
    {
        var result = ((IDictionary<object, object>?)null).AsReadOnlyDictionary();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void AsReadOnlyCollectionShouldWorkCorrectly()
    {
        var standardCollection = TestsHelper.GetStandardCollection();
        var standardDictionary = TestsHelper.GetElementsMap(standardCollection);
        var readonlyDictionary = standardDictionary.AsReadOnlyDictionary();

        Assert.Equal(standardDictionary, readonlyDictionary);
    }

    [Fact]
    public void MapSafelyShouldHandleNull()
    {
        var result = ((IEnumerable<object>?)null).MapSafely(x => 1, x => 2);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void MapSafelyShouldThrowExceptionIfTheProvidedKeySelectorIsNull()
    {
        var standardCollection = TestsHelper.GetStandardCollection();
        Assert.Throws<ArgumentNullException>(() => standardCollection.MapSafely<int, int, int>(null!, x => x));
    }

    [Fact]
    public void MapSafelyShouldThrowExceptionIfTheProvidedValueSelectorIsNull()
    {
        var standardCollection = TestsHelper.GetStandardCollection();
        Assert.Throws<ArgumentNullException>(() => standardCollection.MapSafely<int, int, int>(x => x, null!));
    }

    [Fact]
    public void MapSafelyShouldHandleNullKeys()
    {
        var standardCollection = TestsHelper.GetStandardCollection();
        var mapResult = standardCollection.MapSafely<int, string, int>(x => null!, x => x);
        Assert.NotNull(mapResult);
        Assert.Empty(mapResult);
    }

    [Fact]
    public void MapSafelyShouldHandleDuplicateKeys()
    {
        var standardCollection = TestsHelper.GetStandardCollection().ToArray();
        var randomKey = RandomizationHelper.GetRandomString();
        var mapResult = standardCollection.MapSafely(_ => randomKey, x => x);
        Assert.NotNull(mapResult);

        var singleRecord = Assert.Single(mapResult);
        Assert.Equal(randomKey, singleRecord.Key);
        Assert.Equal(standardCollection[0], singleRecord.Value);
    }

    [Fact]
    public void MapSafelyShouldWorkCorrectly()
    {
        var standardCollection = TestsHelper.GetStandardCollection().ToArray();
        var mapResult = standardCollection.MapSafely(x => x, DoubleNumber);
        Assert.Equal(standardCollection.Length, mapResult.Count);

        foreach (var el in standardCollection)
        {
            Assert.True(mapResult.ContainsKey(el));
            Assert.Equal(DoubleNumber(el), mapResult[el]);
        }

        static int DoubleNumber(int t) => t * 2;
    }
}
