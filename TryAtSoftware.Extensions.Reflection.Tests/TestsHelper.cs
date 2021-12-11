namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using Xunit;

public static class TestsHelper
{
    public static void AssertSameMember(this MemberInfo memberInfo, Type declaringType, string memberName)
    {
        Assert.NotNull(memberInfo);
        Assert.Equal(memberName, memberInfo.Name);
        Assert.Equal(declaringType, memberInfo.DeclaringType);
    }
}