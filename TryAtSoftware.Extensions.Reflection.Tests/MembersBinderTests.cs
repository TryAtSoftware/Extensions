namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Tests.Models;
using Xunit;

public class MembersBinderTests
{
    private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

    [Fact]
    public void TheNonGenericMembersBinderShouldRequireProvidedType() => Assert.Throws<ArgumentNullException>(() => new MembersBinder(null!, MemberIsValid, DefaultBindingFlags));

    [Fact]
    public void MemberBinderShouldBeInitializedCorrectlyWithDifferentKeySelector()
    {
        var personMembersBinder = new MembersBinder<Person>(MemberIsValid, KeySelector, DefaultBindingFlags);
        Assert.NotNull(personMembersBinder.MemberInfos);

        Assert.Equal(4, personMembersBinder.MemberInfos.Count);

        var firstNameKey = ChangeName(nameof(Person.FirstName));
        Assert.True(personMembersBinder.MemberInfos.TryGetValue(firstNameKey, out var firstNameMemberInfo));
        Assert.NotNull(firstNameMemberInfo);
        firstNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.FirstName));

        var middleNameKey = ChangeName(nameof(Person.MiddleName));
        Assert.True(personMembersBinder.MemberInfos.TryGetValue(middleNameKey, out var middleNameMemberInfo));
        Assert.NotNull(middleNameMemberInfo);
        middleNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.MiddleName));

        var lastNameKey = ChangeName(nameof(Person.LastName));
        Assert.True(personMembersBinder.MemberInfos.TryGetValue(lastNameKey, out var lastNameMemberInfo));
        Assert.NotNull(lastNameMemberInfo);
        lastNameMemberInfo.AssertSameMember(typeof(Person), nameof(Person.LastName));

        var ageKey = ChangeName(nameof(Person.Age));
        Assert.True(personMembersBinder.MemberInfos.TryGetValue(ageKey, out var ageMemberInfo));
        Assert.NotNull(ageMemberInfo);
        ageMemberInfo.AssertSameMember(typeof(Person), nameof(Person.Age));

        string KeySelector(MemberInfo member) => ChangeName(member.Name);
        string ChangeName(string memberName) => $"M_{memberName}";
    }
    
    [Fact]
    public void MemberBinderShouldBeInitializedCorrectly()
    {
        var personMembersBinder = new MembersBinder<Person>(MemberIsValid, BindingFlags.Public | BindingFlags.Instance);

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
        var personMembersBinder = new MembersBinder<Student>(MemberIsValid, DefaultBindingFlags | BindingFlags.FlattenHierarchy);

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

    private static bool MemberIsValid(MemberInfo memberInfo) => memberInfo is PropertyInfo { CanWrite: true };
}