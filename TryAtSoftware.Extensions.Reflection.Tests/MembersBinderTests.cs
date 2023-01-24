namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Tests.Models;
using TryAtSoftware.Extensions.Reflection.Tests.Models.Interfaces;
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

        Assert.NotNull(personMembersBinder.MemberInfos);
        Assert.Equal(4, personMembersBinder.MemberInfos.Count);
        Assert.Equal(typeof(Person), personMembersBinder.Type);

        var firstNameKey = ChangeName(nameof(Person.FirstName));
        personMembersBinder.AssertMemberExists(typeof(Person), firstNameKey, nameof(Person.FirstName));

        var middleNameKey = ChangeName(nameof(Person.MiddleName));
        personMembersBinder.AssertMemberExists(typeof(Person), middleNameKey, nameof(Person.MiddleName));

        var lastNameKey = ChangeName(nameof(Person.LastName));
        personMembersBinder.AssertMemberExists(typeof(Person), lastNameKey, nameof(Person.LastName));

        var ageKey = ChangeName(nameof(Person.Age));
        personMembersBinder.AssertMemberExists(typeof(Person), ageKey, nameof(Person.Age));

        string KeySelector(MemberInfo member) => ChangeName(member.Name);
        string ChangeName(string memberName) => $"M_{memberName}";
    }
    
    [Fact]
    public void MemberBinderShouldBeInitializedCorrectly()
    {
        var personMembersBinder = new MembersBinder<Person>(MemberIsValid, DefaultBindingFlags);

        Assert.NotNull(personMembersBinder.MemberInfos);
        Assert.Equal(4, personMembersBinder.MemberInfos.Count);
        Assert.Equal(typeof(Person), personMembersBinder.Type);

        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.FirstName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.MiddleName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.LastName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.Age));
    }

    [Fact]
    public void MemberBinderShouldBeInitializedCorrectlyForDerivedTypes()
    {
        var personMembersBinder = new MembersBinder<Student>(MemberIsValid, DefaultBindingFlags);

        Assert.NotNull(personMembersBinder.MemberInfos);
        Assert.Equal(5, personMembersBinder.MemberInfos.Count);
        Assert.Equal(typeof(Student), personMembersBinder.Type);

        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.FirstName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.MiddleName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.LastName));
        personMembersBinder.AssertMemberExists(typeof(Person), nameof(Person.Age));
        personMembersBinder.AssertMemberExists(typeof(Student), nameof(Student.School));
    }

    [Fact]
    public void MembersBinderShouldIncludedMembersFromExtendedInterfaces()
    {
        var trackableMembersBinder = new MembersBinder<ITrackable>(memberInfo => memberInfo.MemberType == MemberTypes.Property, DefaultBindingFlags);
        
        Assert.NotNull(trackableMembersBinder.MemberInfos);
        Assert.Equal(5, trackableMembersBinder.MemberInfos.Count);
        Assert.Equal(typeof(ITrackable), trackableMembersBinder.Type);
        
        trackableMembersBinder.AssertMemberExists(typeof(IIdentifiable), nameof(IIdentifiable.Id));
        trackableMembersBinder.AssertMemberExists(typeof(ITrackable), nameof(ITrackable.CreatedBy));
        trackableMembersBinder.AssertMemberExists(typeof(ITrackable), nameof(ITrackable.CreatedAt));
        trackableMembersBinder.AssertMemberExists(typeof(ITrackable), nameof(ITrackable.LastModifiedBy));
        trackableMembersBinder.AssertMemberExists(typeof(ITrackable), nameof(ITrackable.LastModifiedAt));
    }

    private static bool MemberIsValid(MemberInfo memberInfo) => memberInfo is PropertyInfo { CanWrite: true };
}