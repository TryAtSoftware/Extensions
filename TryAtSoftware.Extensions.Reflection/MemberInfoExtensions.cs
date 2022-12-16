namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Reflection;

/// <summary>
/// A static class containing standard extension methods applicable to any <see cref="MemberInfo"/> instance.
/// </summary>
public static class MemberInfoExtensions
{
    /// <summary>
    /// Use this method to retrieve the value of a member info for a given instnce.
    /// </summary>
    /// <param name="memberInfo">The extended <see cref="MemberInfo"/> instance.</param>
    /// <param name="instance">The object whose member value will be returned.</param>
    /// <returns>Returns the value for the extended <paramref name="memberInfo"/> and the provided <paramref name="instance"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="memberInfo"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a value for the provided <paramref name="memberInfo"/> could not be resolved.</exception>
    public static object? GetValue(this MemberInfo memberInfo, object? instance)
    {
        if (memberInfo is null) throw new ArgumentNullException(nameof(memberInfo));

        return memberInfo.MemberType switch
        {
            MemberTypes.Property when memberInfo is PropertyInfo { CanRead: true } propertyInfo => propertyInfo.GetValue(instance),
            MemberTypes.Field when memberInfo is FieldInfo fieldInfo => fieldInfo.GetValue(instance),
            _ => throw new InvalidOperationException($"MemberInfo must be a readable property or a field in order to get its value.")
        };
    }
}