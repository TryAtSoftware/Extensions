namespace TryAtSoftware.Extensions.Collections.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class DictionaryExtensionsTests
{
    [Fact]
    public void OrEmptyIfNullShouldReturnTheSameDictionaryIfItIsNotNull()
    {
        var collection = TestsHelper.GetStandardCollection();
        var dictionary = TestsHelper.GetElementsMap(collection);

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
}
