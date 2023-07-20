namespace TryAtSoftware.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class FluidDictionary<TKey> : IFluidDictionary<TKey>
{
    private Dictionary<TKey, object> _dictionary;

    public FluidDictionary()
    {
        _dictionary = new Dictionary<TKey, object>();
    }

    public bool Set<T>(TKey key, T value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        _dictionary[key] = value;
        return true;
    }

    public bool TryGetValue<T>(TKey key, out T value)
    {
        if (_dictionary.TryGetValue(key, out var objValue) && objValue is T)
        {
            value = (T)objValue;
            return true;
        }

        value = default(T);
        return false;
    }

    public T GetValueOrDefault<T>(TKey key)
    {
        if (_dictionary.TryGetValue(key, out var objValue))
        {
            if (objValue is T typedValue)
            {
                return typedValue;
            }
        }

        return default(T);
    }

    public T GetRequiredValue<T>(TKey key)
    {
        if (!_dictionary.TryGetValue(key, out var objValue))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the dictionary.");
        }

        if (!(objValue is T))
        {
            throw new InvalidCastException($"Value for key '{key}' is not of type '{typeof(T).Name}'.");
        }

        return (T)objValue;
    }

    public IEnumerable<TKey> Keys => _dictionary.Keys;

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return _dictionary.Remove(key);
    }
}
