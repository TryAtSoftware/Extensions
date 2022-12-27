namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;

public static class TypeExtensions
{
    /// <summary>
    /// Use this method to extract in a dictionary the generic types setup for a given type.
    /// </summary>
    /// <param name="type">The type used to extract the generic types setup from.</param>
    /// <param name="typesConfiguration">A one-to-one mapping of "attribute type" and "generic argument type".</param>
    /// <returns>Returns a dictionary containing the specific types for each of the generic arguments against their names.</returns>
    /// <remarks>
    /// Imagine we have the following type definition:
    /// public class MyType&lt;[KeyType] TKey, [ValueType] TValue&gt; {}
    ///
    /// ExtractGenericParametersSetup(typeof(MyType), { typeof(KeyTypeAttribute): typeof(int), typeof(ValueTypeAttribute): typeof(string) })
    /// should return { "TKey": typeof(int), "TValue": typeof(string) }
    /// </remarks>
    public static IDictionary<string, Type> ExtractGenericParametersSetup(this Type type, IDictionary<Type, Type> typesConfiguration)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (typesConfiguration is null) throw new ArgumentNullException(nameof(typesConfiguration));

        var dict = new Dictionary<string, Type>();
        if (!type.ContainsGenericParameters) return dict;

        foreach (var genericArgument in type.GetGenericArguments())
        {
            var attributes = genericArgument.CustomAttributes.ToArray();
            if (attributes.Length > 1) throw new InvalidOperationException($"There are more than one attributes defined for a generic parameter [{genericArgument.Name}] of the automatically registered component of type {TypeNames.Get(type)}");
            if (attributes.Length == 0) throw new InvalidOperationException($"Generic parameter could not be resolved for automatically registered component of type {TypeNames.Get(type)}.");

            var attributeType = attributes[0].AttributeType;
            if (!typesConfiguration.TryGetValue(attributeType, out var resolvedGenericType) || resolvedGenericType is null)
                throw new InvalidOperationException($"There was no provided type for the {attributeType}.");

            dict[genericArgument.Name] = resolvedGenericType;
        }

        return dict;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericParametersSetup"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Type MakeGenericType(this Type type, IDictionary<string, Type> genericParametersSetup)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (genericParametersSetup is null) throw new ArgumentNullException(nameof(genericParametersSetup));

        if (!type.ContainsGenericParameters) return type;

        var genericTypes = new List<Type>();
        foreach (var genericArgument in type.GetGenericArguments())
        {
            Type currentResolvedType;
            if (genericArgument.IsGenericParameter && genericParametersSetup.TryGetValue(genericArgument.Name, out var resolvedType))
                currentResolvedType = resolvedType;
            else
                currentResolvedType = MakeGenericType(genericArgument, genericParametersSetup);

            genericTypes.Add(currentResolvedType);
        }

        var genericDefinition = type.GetGenericTypeDefinition();
        return genericDefinition.MakeGenericType(genericTypes.ToArray());
    }
}
