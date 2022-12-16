namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Reflection;

public static class MemberInfoExtensions
{
    public static object? GetValue(this MemberInfo memberInfo, object? instance)
    {
        if (memberInfo is null) throw new ArgumentNullException(nameof(memberInfo));

        return memberInfo.MemberType switch
        {
            MemberTypes.Property when memberInfo is PropertyInfo { CanRead: true } propertyInfo => propertyInfo.GetValue(instance),
            MemberTypes.Field when memberInfo is FieldInfo fieldInfo => fieldInfo.GetValue(instance),
            _ => throw new ArgumentException($"MemberInfo must be a readable property or a field in order to get its value.", nameof(memberInfo))
        };
    }
}