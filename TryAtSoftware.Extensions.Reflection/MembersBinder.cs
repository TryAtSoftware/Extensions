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
        : this(isValid, keySelector: null, bindingFlags)
    {
    }
    
    public MembersBinder([CanBeNull] Func<MemberInfo, bool> isValid, [CanBeNull] Func<MemberInfo, string> keySelector, BindingFlags bindingFlags)
    {
        var members = GetMembers(typeof(TEntity), isValid, bindingFlags, keySelector);
        this.MemberInfos = new ReadOnlyDictionary<string, MemberInfo>(members);
    }

    public IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }

    private static Dictionary<string, MemberInfo> GetMembers([NotNull] IReflect type, [CanBeNull] Func<MemberInfo, bool> isValid, BindingFlags bindingFlags, [CanBeNull] Func<MemberInfo, string> keySelector)
    {
        var membersDict = new Dictionary<string, MemberInfo>();
        var members = type.GetMembers(bindingFlags).SafeWhere(isValid);

        foreach (var member in members)
        {
            string key;
            if (keySelector is null) key = member.Name;
            else 
            {
                key = keySelector(member);
                if (string.IsNullOrWhiteSpace(key)) continue;
            }

            membersDict[key] = member;
        }

        return membersDict;
    }
}