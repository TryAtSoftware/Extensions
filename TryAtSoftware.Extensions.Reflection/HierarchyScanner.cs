namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// An implementation of the <see cref="IHierarchyScanner"/> interface.
/// It is responsible for extracting a single attribute from any layer of the type hierarchy.
/// </summary>
public class HierarchyScanner : IHierarchyScanner
{
    /// <inheritdoc />
    public IReadOnlyCollection<TAttribute> ScanForAttribute<TAttribute>(MemberInfo memberInfo)
        where TAttribute : Attribute
    {
        if (memberInfo is null) throw new ArgumentNullException(nameof(memberInfo));

        var attributesStack = new Stack<TAttribute>();

        var directAttribute = memberInfo.GetCustomAttribute<TAttribute>(inherit: true);
        if (directAttribute is not null) attributesStack.Push(directAttribute);

        if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.ReflectedType is not null)
        {
            var classAttribute = memberInfo.ReflectedType.GetCustomAttribute<TAttribute>(inherit: true);
            if (classAttribute is not null) attributesStack.Push(classAttribute);
        }

        var result = new List<TAttribute>(capacity: attributesStack.Count);
        while (attributesStack.Count != 0) result.Add(attributesStack.Pop());

        return result.AsReadOnly();
    }
}