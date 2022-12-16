namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// A standard implementation of the <see cref="IMembersBinder"/> interface.
/// </summary>
public class MembersBinder : IMembersBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MembersBinder"/> class.
    /// </summary>
    /// <param name="type">The type used by this member binder.</param>
    /// <param name="isValid">A function defining whether or not a given member should be managed by this instance.</param>
    /// <param name="bindingFlags">The binding flags used to control the member search.</param>
    public MembersBinder(Type type, Func<MemberInfo, bool>? isValid, BindingFlags bindingFlags)
        : this(type, isValid, keySelector: null, bindingFlags)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MembersBinder"/> class.
    /// </summary>
    /// <param name="type">The type used by this member binder.</param>
    /// <param name="isValid">A function defining whether or not a given member should be managed by this instance.</param>
    /// <param name="keySelector">A function that should map each discovered <see cref="MemberInfo"/> instance to unique key. If this is null, the member's name will be used (which should be avoided when possible).</param>
    /// <param name="bindingFlags">The binding flags used to control the member search.</param>
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

/// <summary>
/// A standard generc implementation of the <see cref="IMembersBinder"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type used by this member binder.</typeparam>
public class MembersBinder<TEntity> : MembersBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MembersBinder{TEntity}"/> class.
    /// </summary>
    /// <param name="isValid">A function defining whether or not a given member should be managed by this instance.</param>
    /// <param name="bindingFlags">The binding flags used to control the member search.</param>
    public MembersBinder(Func<MemberInfo, bool>? isValid, BindingFlags bindingFlags)
        : this(isValid, keySelector: null, bindingFlags)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MembersBinder{TEntity}"/> class.
    /// </summary>
    /// <param name="isValid">A function defining whether or not a given member should be managed by this instance.</param>
    /// <param name="keySelector">A function that should map each discovered <see cref="MemberInfo"/> instance to unique key. If this is null, the member's name will be used (which should be avoided when possible).</param>
    /// <param name="bindingFlags">The binding flags used to control the member search.</param>
    public MembersBinder(Func<MemberInfo, bool>? isValid, Func<MemberInfo, string>? keySelector, BindingFlags bindingFlags)
        : base(typeof(TEntity), isValid, keySelector, bindingFlags)
    {
    }
}