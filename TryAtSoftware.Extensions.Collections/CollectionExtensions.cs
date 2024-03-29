namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A static class containing standard extension methods applicable to any <see cref="IEnumerable{T}"/> instance.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Use this method to filter the elements of the <paramref name="collection"/> safely in terms of the <paramref name="filter"/> nullability.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <param name="filter">A function to test each element for a condition.</param>
    /// <returns>Returns the same collection if the provided <paramref name="filter"/> is null. Else, returns the filtered collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="collection"/> is null.</exception>
    public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> collection, Func<T, bool>? filter)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        if (filter is null) return collection;
        return collection.Where(filter);
    }

    /// <summary>
    /// This method will give you an empty enumerable instance if the extended <paramref name="collection"/> is null.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <returns>Returns the same collection if it was not null. Else, returns an empty enumerable.</returns>
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? collection) => collection ?? Enumerable.Empty<T>();

    /// <summary>
    /// Use this method to filter out all null values from the extended <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}"/> containing all elements from the extended <paramref name="collection"/> that are not null in the same order.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="collection"/> is null.</exception>
    public static IEnumerable<T> IgnoreNullValues<T>(this IEnumerable<T?> collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));
        return collection.Where(x => x is not null)!;
    }

    /// <summary>
    /// Use this method to filter out all values that are null, empty or consist of whitespace characters only from the extended <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}"/> containing all elements from the extended <paramref name="collection"/> that are not null or empty and do not consist of whitespace characters only.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="collection"/> is null.</exception>
    public static IEnumerable<string> IgnoreNullOrWhitespaceValues(this IEnumerable<string?> collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));
        return collection.Where(x => !string.IsNullOrWhiteSpace(x))!;
    }
    
    /// <summary>
    /// Use this method to filter out all default values from the extended <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}"/> containing all elements from the extended <paramref name="collection"/> that do not equal the default value for <typeparamref name="T"/> in the same order.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="collection"/> is null.</exception>
    public static IEnumerable<T> IgnoreDefaultValues<T>(this IEnumerable<T> collection)
        where T : struct
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        var equalityComparer = EqualityComparer<T>.Default;
        return collection.Where(x => !equalityComparer.Equals(x, default));
    }

    /// <summary>
    /// Use this method to safely concatenate two <see cref="IEnumerable{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <param name="otherCollection">The other <see cref="IEnumerable{T}"/> instance to concatenate.</param>
    /// <returns>Returns an <see cref="IEnumerable{T}"/> containing all elements from <paramref name="collection"/> and all elements from <paramref name="otherCollection"/> in the same order.</returns>
    public static IEnumerable<T> ConcatenateWith<T>(this IEnumerable<T>? collection, IEnumerable<T>? otherCollection)
    {
        foreach (var item in collection.OrEmptyIfNull()) yield return item;
        foreach (var item in otherCollection.OrEmptyIfNull()) yield return item;
    }

    /// <summary>
    /// Use this method to produce the union of multiple <see cref="HashSet{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="collections">The extended collection of <see cref="IEnumerable{T}"/> instances.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains the elements that form the union of the extended collections.</returns>
    public static HashSet<T> Union<T>(this IEnumerable<IEnumerable<T>?>? collections)
    {
        var compoundSet = new HashSet<T>();
        foreach (var collection in collections.OrEmptyIfNull().IgnoreNullValues())
        {
            foreach (var value in collection)
                compoundSet.Add(value);
        }

        return compoundSet;
    }

    /// <summary>
    /// Use this method to construct an <see cref="IReadOnlyCollection{T}"/> instance from the extended <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The extended <see cref="IEnumerable{T}"/> instance.</param>
    /// <returns>Returns an <see cref="IReadOnlyCollection{T}"/> containing all elements from the extended <paramref name="collection"/> in the same order.</returns>
    public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T>? collection) => collection.OrEmptyIfNull().ToList().AsReadOnly();
}