namespace TryAtSoftware.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class FluidDictionary<TKey> : IFluidDictionary<TKey>
{
    private readonly Dictionary<TKey, object> _dictionary;

    /// <summary>
    /// constructor
    /// </summary>
    public FluidDictionary()
    {
        this._dictionary = new Dictionary<TKey, object>();
    }

    /// <summary>
    /// Setting a value against a particular key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Set<T>(TKey key, T value) 
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        this._dictionary[key] = value;
        return true;
    }
    /// <summary>
    /// Fetching a value using key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue<T>(TKey key, out T value)
    {
        if (this._dictionary.TryGetValue(key, out var objValue) && objValue is T)
        {
            value = (T)objValue;
            return true;
        }

        value = default;
        return false;
    }
    /// <summary>
    /// Fetch the value or default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetValueOrDefault<T>(TKey key)
    {
        if (this._dictionary.TryGetValue(key, out var objValue))
        {
            if (objValue is T typedValue)
            {
                return typedValue;
            }
        }

        return default;
    }
    /// <summary>
    /// Handling the exception for GetRequiredValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="InvalidCastException"></exception>
    public T GetRequiredValue<T>(TKey key)
    {
        if (!this._dictionary.TryGetValue(key, out var objValue))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the dictionary.");
        }

        if (!(objValue is T))
        {
            throw new InvalidCastException($"Value for key '{key}' is not of type '{typeof(T).Name}'.");
        }

        return (T)objValue;
    }

    public IEnumerable<TKey> Keys => this._dictionary.Keys;
    /// <summary>
    /// Removing value from dictionary object using key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return this._dictionary.Remove(key);
    }
}
