namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> collection, Func<T, bool>? filter)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        if (filter is null) return collection;
        return collection.Where(filter);
    }

    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? collection) => collection ?? Enumerable.Empty<T>();

    public static IEnumerable<T> IgnoreNullValues<T>(this IEnumerable<T> collection)
        where T : class
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));
        return collection.Where(x => x is not null);
    }

    public static IEnumerable<T> IgnoreDefaultValues<T>(this IEnumerable<T> collection)
        where T : struct, IEquatable<T>
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));
        return collection.Where(x => !x.Equals(default));
    }

    public static IEnumerable<T> ConcatenateWith<T>(this IEnumerable<T>? collection, IEnumerable<T>? otherCollection)
    {
        foreach (var item in collection.OrEmptyIfNull()) yield return item;
        foreach (var item in otherCollection.OrEmptyIfNull()) yield return item;
    }
}