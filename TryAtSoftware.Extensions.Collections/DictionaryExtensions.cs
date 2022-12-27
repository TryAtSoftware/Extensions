namespace TryAtSoftware.Extensions.Collections;

using System.Collections.Generic;

/// <summary>
/// A static class containing standard extension methods applicable to any <see cref="IDictionary{TKey,TValue}"/> instance.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// This method will give you an empty dictionary instance if the extended <paramref name="dictionary"/> is null.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}"/> instance.</param>
    /// <returns>Returns the same dictionary if it was not null. Else, returns an new empty dictionary.</returns>
    public static IDictionary<TKey, TValue> OrEmptyIfNull<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary) => dictionary ?? new Dictionary<TKey, TValue>(capacity: 0);
}