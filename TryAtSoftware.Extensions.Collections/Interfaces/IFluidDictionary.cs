namespace TryAtSoftware.Extensions.Collections.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

public interface IFluidDictionary<TKey>
{
    bool Set<T>(TKey key, T value);
    bool TryGetValue<T>(TKey key, out T value);
    T GetValueOrDefault<T>(TKey key);
    T GetRequiredValue<T>(TKey key);
    IEnumerable<TKey> Keys { get; }
}