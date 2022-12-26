namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

/// <summary>
/// A static class responsible for providing beautified type names.
/// </summary>
/// <remarks>
/// Internally, there is a cache so multiple calls to the same method will not be slow.
/// </remarks>
public static class TypeNames
{
    private const char GenericTypeNameSeparator = '`';
    private static readonly ConcurrentDictionary<Type, string> _memo = new();

    /// <summary>
    /// Use this method to get the beautified name of the requested <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which a beautified name should be generated and returned.</param>
    /// <returns>Returns the subsequently generated beautified name of the provided <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="type"/> is null.</exception>
    public static string Get(Type type)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
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

    private static string SanitizeTypeName(Type type)
    {
        var originalTypeName = type.Name;
        if (!type.IsGenericType) return originalTypeName;

        return originalTypeName[..originalTypeName.IndexOf(GenericTypeNameSeparator)];
    }
}

/// <summary>
/// A static class responsible for providing beautified type names.
/// </summary>
/// <typeparam name="T">The type for which a beautified name is requested.</typeparam>
/// <remarks>
/// Internally, the non-generic <see cref="TypeNames"/> class is used.
/// </remarks>
public static class TypeNames<T>
{
    /// <summary>
    /// Gets the beautified name of <typeparamref name="T"/>.
    /// </summary>
    public static string Value => TypeNames.Get(typeof(T));
}