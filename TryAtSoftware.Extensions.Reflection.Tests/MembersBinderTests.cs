namespace TryAtSoftware.Extensions.Reflection.Tests;

using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Tests.Types;
using Xunit;

public class MembersBinderTests
{
    [Fact]
    public void MemberBinderShouldBeInitializedCorrectly()
    {
        var personMembersBinder = new MembersBinder<Person>(x => x is PropertyInfo { CanWrite: true }, BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(personMembersBinder.MemberInfos);

        Assert.Equal(4, personMembersBinder.MemberInfos.Count);

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.FirstName), out var firstNameMemberInfo));
        Assert.NotNull(firstNameMemberInfo);
        firstNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.FirstName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.MiddleName), out var middleNameMemberInfo));
        Assert.NotNull(middleNameMemberInfo);
        middleNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.MiddleName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.LastName), out var lastNameMemberInfo));
        Assert.NotNull(lastNameMemberInfo);
        lastNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.LastName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.Age), out var ageMemberInfo));
        Assert.NotNull(ageMemberInfo);
        ageMemberInfo.AssertSameMember(typeof(Person), nameof(Person.Age));
    }

    [Fact]
    public void MemberBinderShouldBeInitializedCorrectlyForDerivedTypes()
    {
        var personMembersBinder = new MembersBinder<Student>(x => x is PropertyInfo { CanWrite: true }, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        Assert.NotNull(personMembersBinder.MemberInfos);

        Assert.Equal(5, personMembersBinder.MemberInfos.Count);

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.FirstName), out var firstNameMemberInfo));
        Assert.NotNull(firstNameMemberInfo);
        firstNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.FirstName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.MiddleName), out var middleNameMemberInfo));
        Assert.NotNull(middleNameMemberInfo);
        middleNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.MiddleName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.LastName), out var lastNameMemberInfo));
        Assert.NotNull(lastNameMemberInfo);
        lastNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.LastName));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Person.Age), out var ageMemberInfo));
        Assert.NotNull(ageMemberInfo);
        ageMemberInfo.AssertSameMember(typeof(Person), nameof(Person.Age));

        Assert.True(personMembersBinder.MemberInfos.TryGetValue(nameof(Student.School), out var schoolMemberInfo));
        Assert.NotNull(schoolMemberInfo);
        schoolMemberInfo.AssertSameMember(typeof(Student), nameof(Student.School));
    }
}