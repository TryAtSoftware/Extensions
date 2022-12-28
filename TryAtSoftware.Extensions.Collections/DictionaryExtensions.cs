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

    /// <summary>
    /// Use this method to form a mapping between the selected keys and values for each element in the extended <paramref name="collection"/>.
    /// This method is `safe` in terms of what happens whenever two elements have the same key - no exception is thrown and the original value remains unchanged.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of keys in the dictionary to form.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary to form.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <param name="keySelector">A function that should select the key for each <typeparamref name="TSource"/> instance from the extended collection.</param>
    /// <param name="valueSelector">A function that should select the value for each <typeparamref name="TSource"/> instance from the extended collection.</param>
    /// <returns>Returns an <see cref="IDictionary{TKey, TValue}"/> instance formed by using the values of the extended <paramref name="collection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="keySelector"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="valueSelector"/> is null.</exception>
    public static IDictionary<TKey, TValue> MapSafely<TSource, TKey, TValue>(this IEnumerable<TSource?>? collection, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector is null) throw new ArgumentNullException(nameof(valueSelector));

        var dictionary = new Dictionary<TKey, TValue>();
        foreach (var sourceElement in collection.OrEmptyIfNull().IgnoreNullValues())
        {
            var key = keySelector(sourceElement);
            if (key is null || dictionary.ContainsKey(key)) continue;

            var value = valueSelector(sourceElement);
            dictionary[key] = value;
        }

        return dictionary;
    }
}