namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// A static class containing standard extension methods that are useful when working with reflection over generic types or methods.
/// </summary>
public static class GenericExtensions
{
    /// <summary>
    /// Use this method to extract in a dictionary the setup of generic parameters for a given type.
    /// </summary>
    /// <param name="type">The type used to extract the generic types setup from.</param>
    /// <param name="typesMap">A one-to-one mapping of "attribute type" and "generic parameter type".</param>
    /// <returns>Returns a dictionary containing the specific types for each of the generic parameters against their names.</returns>
    /// <remarks>
    /// Imagine we have the following type definition:
    /// public class MyType&lt;[KeyType] TKey, [ValueType] TValue&gt; {}
    ///
    /// ExtractGenericParametersSetup(typeof(MyType), { typeof(KeyTypeAttribute): typeof(int), typeof(ValueTypeAttribute): typeof(string) })
    /// should return { "TKey": typeof(int), "TValue": typeof(string) }
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="type"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="typesMap"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if there are none or more than one attributes for any of the unresolved generic parameters.</exception>
    /// <exception cref="InvalidOperationException">Thrown if there is no configured type for the attribute a generic parameter is decorated with.</exception>
    public static IDictionary<string, Type> ExtractGenericParametersSetup(this Type type, IDictionary<Type, Type> typesMap)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (typesMap is null) throw new ArgumentNullException(nameof(typesMap));

        var dict = new Dictionary<string, Type>();
        if (!type.ContainsGenericParameters) return dict;

        foreach (var genericArgument in type.GetGenericArguments())
        {
            var attributes = genericArgument.CustomAttributes.Where(x => !IsCompilerGenerated(x.AttributeType)).ToArray();
            if (attributes.Length > 1) throw new InvalidOperationException($"There are more than one attributes defined for a generic parameter [{genericArgument.Name}] of the automatically registered component of type {TypeNames.Get(type)}");
            if (attributes.Length == 0) throw new InvalidOperationException($"Generic parameter could not be resolved for automatically registered component of type {TypeNames.Get(type)}.");

            var attributeType = attributes[0].AttributeType;
            if (!typesMap.TryGetValue(attributeType, out var resolvedGenericType) || resolvedGenericType is null)
                throw new InvalidOperationException($"There was no provided type for the {attributeType}.");

            dict[genericArgument.Name] = resolvedGenericType;
        }

        return dict;
    }

    /// <summary>
    /// Use this method to make the extended <paramref name="type"/> generic using a parameters setup.
    /// </summary>
    /// <param name="type">The type that should be made generic.</param>
    /// <param name="genericParametersSetup">A dictionary containing the specific types for each of the generic parameters against their names.</param>
    /// <returns>Returns the generic version of the extended <paramref name="type"/> where each generic parameter is substituted with a specific type from the <paramref name="genericParametersSetup"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="type"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="genericParametersSetup"/> is null.</exception>
    public static Type MakeGenericType(this Type type, IDictionary<string, Type> genericParametersSetup)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (genericParametersSetup is null) throw new ArgumentNullException(nameof(genericParametersSetup));

        if (!type.ContainsGenericParameters) return type;

        var genericTypes = new List<Type>();
        foreach (var genericArgument in type.GetGenericArguments())
        {
            Type currentResolvedType;
            if (genericArgument.IsGenericParameter)
                currentResolvedType = genericParametersSetup.TryGetValue(genericArgument.Name, out var resolvedType) ? resolvedType : genericArgument;
            else
                currentResolvedType = MakeGenericType(genericArgument, genericParametersSetup);

            genericTypes.Add(currentResolvedType);
        }

        var genericDefinition = type.GetGenericTypeDefinition();
        return genericDefinition.MakeGenericType(genericTypes.ToArray());
    }

    private static bool IsCompilerGenerated(MemberInfo type) => type.GetCustomAttribute<CompilerGeneratedAttribute>() is not null;
}