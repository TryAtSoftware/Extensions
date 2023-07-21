namespace TryAtSoftware.Extensions.Collections;

using System;
using System.Collections.Generic;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class FluidDictionary<TKey> : IFluidDictionary<TKey>
{
    private Dictionary<TKey, object> dictionary;

    public FluidDictionary()
    {
        dictionary = new Dictionary<TKey, object>();
    }

    /// <summary>
    /// Sets the value of type T associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value is successfully set, false otherwise.</returns>
    public bool Set<T>(TKey key, T value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        dictionary[key] = value;
        return true;
    }

    /// <summary>
    /// Attempts to get the value associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, the default value for the type T.</param>
    /// <returns>True if the key is found and the value is of type T, false otherwise.</returns>
    public bool TryGetValue<T>(TKey key, out T value)
    {
        if (dictionary.TryGetValue(key, out var objValue) && objValue is T)
        {
            value = (T)objValue;
            return true;
        }

        value = default(T);
        return false;
    }

    /// <summary>
    /// Gets the value associated with the specified key in the dictionary.
    /// If the key is not found or the value is not of type T, returns the default value of type T.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>The value associated with the specified key, or the default value of type T if the key is not found or the value is not of type T.</returns>
    public T GetValueOrDefault<T>(TKey key)
    {
        if (dictionary.TryGetValue(key, out var objValue))
        {
            if (objValue is T typedValue)
            {
                return typedValue;
            }
        }

        return default(T);
    }

    /// <summary>
    /// Gets the value associated with the specified key in the dictionary.
    /// If the key is not found or the value is not of type T, throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found in the dictionary.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not of type T.</exception>
    public T GetRequiredValue<T>(TKey key)
    {
        if (!dictionary.TryGetValue(key, out var objValue))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the dictionary.");
        }

        if (!(objValue is T))
        {
            throw new InvalidCastException($"Value for key '{key}' is not of type '{typeof(T).Name}'.");
        }

        return (T)objValue;
    }

    /// <summary>
    /// Gets an enumeration of keys in the dictionary.
    /// </summary>
    public IEnumerable<TKey> Keys => dictionary.Keys;

    /// <summary>
    /// Removes the value associated with the specified key from the dictionary.
    /// </summary>
    /// <param name="key">The key whose value to remove.</param>
    /// <returns>True if the value is successfully removed; otherwise, false.</returns>
    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return dictionary.Remove(key);
    }
}
