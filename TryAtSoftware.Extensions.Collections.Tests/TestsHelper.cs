namespace TryAtSoftware.Extensions.Collections.Tests;
using System.Collections.Generic;
using System.Linq;
using Xunit;

internal static class TestsHelper
{
    internal static IEnumerable<int> GetStandardCollection() => Enumerable.Range(0, 5);

    internal static IEnumerable<T> Repeat<T>(IEnumerable<T> collection, int n)
    {
        foreach (var el in collection)
        {
            for (var i = 0; i < n; i++) yield return el;
        }
    }

    internal static IDictionary<T, int> GetElementsMap<T>(IEnumerable<T> collection)
        where T : notnull
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
