namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

public static class TypeNames
{
    private const char GenericTypeNameSeparator = '`';
    private static readonly ConcurrentDictionary<Type, string> _memo = new();

    public static string Get(Type type)
    {
        if (_memo.TryGetValue(type, out var memoizedName)) return memoizedName;

        var sanitizedTypeName = SanitizeTypeName(type);
        var typeNameBuilder = new StringBuilder();
        typeNameBuilder.Append(sanitizedTypeName);

        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments();
            var genericArgumentNames = genericArguments.Select(Get);
            typeNameBuilder.Append('<');
            typeNameBuilder.Append(string.Join(", ", genericArgumentNames));
            typeNameBuilder.Append('>');
        }

        var name = typeNameBuilder.ToString();
        _memo[type] = name;
        return name;
    }

    private static string SanitizeTypeName([NotNull] Type type)
    {
        var originalTypeName = type.Name;
        if (!type.IsGenericType) return originalTypeName;

        return originalTypeName[..originalTypeName.IndexOf(GenericTypeNameSeparator)];
    }
}

public static class TypeNames<T>
{
    public static string Value => TypeNames.Get(typeof(T));
}