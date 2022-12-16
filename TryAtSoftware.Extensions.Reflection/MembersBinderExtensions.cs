namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// A static class containing standard extension methods applicable to any <see cref="IMembersBinder"/> instance.
/// </summary>
public static class MembersBinderExtensions
{
    /// <summary>
    /// Use this method to retrieve a required member from the extended <paramref name="membersBinder"/> instance.
    /// </summary>
    /// <param name="membersBinder">The extended <see cref="IMembersBinder"/> instance.</param>
    /// <param name="key">The key of the required member info.</param>
    /// <returns>Returns the requested member info from the extended <paramref name="membersBinder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="membersBinder"/> is null or the provided <paramref name="key"/> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the extended <paramref name="membersBinder"/> does not contain a member mpped against the requested <paramref name="key"/>.</exception>
    public static MemberInfo GetRequiredMemberInfo(this IMembersBinder membersBinder, string key)
    {
        if (membersBinder is null) throw new ArgumentNullException(nameof(membersBinder));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

        if (!membersBinder.MemberInfos.TryGetValue(key, out var memberInfo))
            throw new InvalidOperationException($"Member {key} not found in {TypeNames.Get(membersBinder.Type)}");

        return memberInfo;
    }
}