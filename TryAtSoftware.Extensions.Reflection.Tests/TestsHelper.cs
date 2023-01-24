namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;
using Xunit;

public static class TestsHelper
{
    public static void AssertMemberExists(this IMembersBinder membersBinder, Type declaringType, string memberName) => membersBinder.AssertMemberExists(declaringType, memberName, memberName);

    public static void AssertMemberExists(this IMembersBinder membersBinder, Type declaringType, string memberKey, string memberName)
    {
        Assert.NotNull(membersBinder);
        Assert.NotNull(declaringType);
        Assert.False(string.IsNullOrWhiteSpace(memberKey));
        Assert.False(string.IsNullOrWhiteSpace(memberName));

        Assert.True(membersBinder.MemberInfos.TryGetValue(memberKey, out var memberInfo));
        Assert.NotNull(memberInfo);
        memberInfo.AssertSameMember(declaringType, memberName);
    }
    
    public static void AssertSameMember(this MemberInfo memberInfo, Type declaringType, string memberName)
    {
        Assert.NotNull(memberInfo);
        Assert.NotNull(declaringType);
        Assert.False(string.IsNullOrWhiteSpace(memberName));

        Assert.Equal(memberName, memberInfo.Name);
        Assert.Equal(declaringType, memberInfo.DeclaringType);
    }
}