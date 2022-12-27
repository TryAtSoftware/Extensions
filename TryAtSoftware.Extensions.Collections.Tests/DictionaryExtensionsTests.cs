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
}
