namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    public static IDictionary<TKey, TValue> OrEmptyIfNull<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary)
        where TKey : notnull
        => dictionary ?? new Dictionary<TKey, TValue>(capacity: 0);

    /// <summary>
    /// Use this method to ensure that a value within a dictionary exists for the requested key.
    /// If such does not exist, a new instance will be created and stored for the requested key.
    /// </summary>
    /// <param name="dictionary">The extended <see cref="IDictionary{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key used to lookup whether or not such value exists in the requested <paramref name="dictionary"/>.</param>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>Returns the value for the requested key.</returns>
    public static TValue EnsureValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        where TValue : class, new()
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (dictionary.TryGetValue(key, out var value)) return value;

        var newValueInstance = new TValue();
        dictionary[key] = newValueInstance;
        return newValueInstance;
    }

    /// <summary>
    /// Use this method to construct an <see cref="IReadOnlyDictionary{TKey, TValue}"/> instance from the extended <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The extended <see cref="IDictionary{TKey, TValue}"/> instance.</param>
    /// <returns>Returns an <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all elements from the extended <paramref name="dictionary"/>.</returns>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary)
        where TKey : notnull
        => new ReadOnlyDictionary<TKey, TValue>(dictionary.OrEmptyIfNull());
}