namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;
using TryAtSoftware.Extensions.Reflection.Tests.Types;
using Xunit;

public class MembersBinderExtensionsTests
{
    [Fact]
    public void GetRequiredMemberInfoShouldWorkCorrectly()
    {
        var personMembersBinder = new MembersBinder<Person>(x => x.MemberType == MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(personMembersBinder.MemberInfos);

        var firstNameMember = personMembersBinder.GetRequiredMemberInfo(nameof(Person.FirstName));
        Assert.NotNull(firstNameMember);

        Assert.Throws<InvalidOperationException>(() => personMembersBinder.GetRequiredMemberInfo("invalid_member_name"));
    }

    [Fact]
    public void GetRequiredMemberInfoShouldHandleInvalidMembersBinderParameter()
    {
        IMembersBinder<Person> membersBinder = null;
        Assert.Throws<ArgumentNullException>(() => membersBinder.GetRequiredMemberInfo(nameof(Person.FirstName)));
    }

    [Fact]
    public void GetRequiredMemberInfoShouldHandleInvalidMemberNameParameter()
    {
        var personMembersBinder = new MembersBinder<Person>(x => x.MemberType == MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);
        Assert.Throws<ArgumentNullException>(() => personMembersBinder.GetRequiredMemberInfo(string.Empty));
    }
}