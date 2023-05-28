namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// An interface defining the structure of a component responsible for scanning type hierarchies.
/// </summary>
/// <remarks>
/// A common application of this interface and its corresponding implementing types is to scan a type hierarchy for a given attribute.
/// </remarks>
public interface IHierarchyScanner
{
    /// <summary>
    /// Use this method to scan the hierarchy associated with the provided <paramref name="memberInfo"/> for attributes of type <typeparamref name="TAttribute"/>.
    /// </summary>
    /// <param name="memberInfo">The <see cref="MemberInfo"/> whose hierarchy should be scanned.</param>
    /// <typeparam name="TAttribute">The concrete attribute type to look for.</typeparam>
    /// <returns>Returns a read-only collection containing all found attribute instances in a hierarchical order.</returns>
    IReadOnlyCollection<TAttribute> Scan<TAttribute>(MemberInfo memberInfo)
        where TAttribute : Attribute;
}