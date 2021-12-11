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
    private readonly Func<MemberInfo, bool> _isValid;
    private readonly BindingFlags _bindingFlags;

    public MembersBinder(Func<MemberInfo, bool> isValid, BindingFlags bindingFlags)
    {
        this._isValid = isValid;
        this._bindingFlags = bindingFlags;

        var members = this.GetMembers(typeof(TEntity));
        this.MemberInfos = new ReadOnlyDictionary<string, MemberInfo>(members);
    }

    public IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }

    private Dictionary<string, MemberInfo> GetMembers([NotNull] IReflect type)
    {
        var membersDict = new Dictionary<string, MemberInfo>();
        var members = type.GetMembers(this._bindingFlags).SafeWhere(this._isValid);

        foreach (var member in members)
            membersDict[member.Name] = member;

        return membersDict;
    }
}