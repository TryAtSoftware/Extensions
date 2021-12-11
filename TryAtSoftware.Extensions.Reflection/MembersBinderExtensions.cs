namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public static class MembersBinderExtensions
{
    public static MemberInfo GetRequiredMemberInfo<T>(this IMembersBinder<T> membersBinder, string name)
    {
        if (membersBinder is null)
            throw new ArgumentNullException(nameof(membersBinder));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (!membersBinder.MemberInfos.TryGetValue(name, out var memberInfo))
            throw new InvalidOperationException($"Member {name} not found in {typeof(T).Name}");

        return memberInfo;
    }
}