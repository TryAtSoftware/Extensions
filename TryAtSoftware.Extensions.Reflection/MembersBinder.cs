namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public class MembersBinder : IMembersBinder
{
    public MembersBinder(Type type, Func<MemberInfo, bool>? isValid, BindingFlags bindingFlags)
        : this(type, isValid, keySelector: null, bindingFlags)
    {
    }
    
    public MembersBinder(Type type, Func<MemberInfo, bool>? isValid, Func<MemberInfo, string>? keySelector, BindingFlags bindingFlags)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        var members = GetMembers(type, isValid, bindingFlags, keySelector);
        this.MemberInfos = new ReadOnlyDictionary<string, MemberInfo>(members);
    }

    /// <inheritdoc />
    public Type Type { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }

    private static Dictionary<string, MemberInfo> GetMembers(IReflect type, Func<MemberInfo, bool>? isValid, BindingFlags bindingFlags, Func<MemberInfo, string>? keySelector)
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

public class MembersBinder<TEntity> : MembersBinder
{
    public MembersBinder(Func<MemberInfo, bool>? isValid, BindingFlags bindingFlags)
        : this(isValid, keySelector: null, bindingFlags)
    {
    }
    
    public MembersBinder(Func<MemberInfo, bool>? isValid, Func<MemberInfo, string>? keySelector, BindingFlags bindingFlags)
        : base(typeof(TEntity), isValid, keySelector, bindingFlags)
    {
    }
}