namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> collection, Func<T, bool> filter)
    {
        if (collection is null)
            throw new ArgumentNullException(nameof(collection));

        if (filter is null)
            return collection;
        return collection.Where(filter);
    }
}