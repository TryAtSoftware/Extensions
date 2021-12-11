namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Linq.Expressions;
using TryAtSoftware.Extensions.Reflection.Tests.Types;
using Xunit;

public class ExpressionsHelperTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheGetMemberInfoMethod() => Assert.Throws<ArgumentNullException>(() => ((Expression<Func<Person, string>>)null).GetMemberInfo());

    [Fact]
    public void MemberInfoShouldBeSuccessfullyRetrieved() => AssertMemberInfoRetrieval<Person, string>(p => p.FirstName, typeof(Person), nameof(Person.FirstName));

    [Fact]
    public void MemberInfoShouldBeSuccessfullyRetrievedFromDerivedClasses() => AssertMemberInfoRetrieval<Student, string>(p => p.FirstName, typeof(Person), nameof(Person.FirstName));

    [Fact]
    public void ExceptionShouldBeThrownIfMemberInfoCannotBeRetrievedSuccessfully()
    {
        Expression<Func<Person, string>> expression = p => p.ToString();
        Assert.Throws<InvalidOperationException>(() => expression.GetMemberInfo());
    }

    private static void AssertMemberInfoRetrieval<T, TValue>(Expression<Func<T, TValue>> selector, Type declaringType, string memberName)
    {
        var memberInfo = selector.GetMemberInfo();
        memberInfo.AssertSameMember(declaringType, memberName);
    }
}