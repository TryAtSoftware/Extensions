namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// An interface defining the structure of a component responsible for discovering and caching information about a subset of the members of a given type.
/// </summary>
public interface IMembersBinder
{
    /// <summary>
    /// Gets the type used by this member binder.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{TKey, TValue}"/> where all discovered <see cref="MemberInfo"/> instances are mapped appropriately to a given key.
    /// </summary>
    IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }
}