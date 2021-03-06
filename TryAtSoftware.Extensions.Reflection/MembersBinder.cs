namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using JetBrains.Annotations;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public class MembersBinder<TEntity> : IMembersBinder<TEntity>
{
    public MembersBinder([CanBeNull] Func<MemberInfo, bool> isValid, BindingFlags bindingFlags)
    {
        var members = GetMembers(typeof(TEntity), isValid, bindingFlags);
        this.MemberInfos = new ReadOnlyDictionary<string, MemberInfo>(members);
    }

    public IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }

    private static Dictionary<string, MemberInfo> GetMembers([NotNull] IReflect type, [CanBeNull] Func<MemberInfo, bool> isValid, BindingFlags bindingFlags)
    {
        var membersDict = new Dictionary<string, MemberInfo>();
        var members = type.GetMembers(bindingFlags).SafeWhere(isValid);

        foreach (var member in members)
            membersDict[member.Name] = member;

        return membersDict;
    }
}