namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Linq.Expressions;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Tests.Randomization;
using TryAtSoftware.Extensions.Reflection.Tests.Types;
using Xunit;

public class ExpressionsExtensionsTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheGetMemberInfoMethod() => Assert.Throws<ArgumentNullException>(() => ((Expression<Func<Person, string>>)null)!.GetMemberInfo());

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

    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheConstructPropertyAccessorMethod() => Assert.Throws<ArgumentNullException>(() => ((PropertyInfo)null)!.ConstructPropertyAccessor<Person, string>());

    [Fact]
    public void PropertyAccessorShouldBeSuccessfullyConstructed()
    {
        var firstNameAccessor = GetCompiledPropertyAccessorInvocationResult<Person, string>(nameof(Person.FirstName));

        var personRandomizer = new PersonRandomizer();
        var person = personRandomizer.PrepareRandomValue();

        var firstName = firstNameAccessor(person);
        Assert.Equal(person.FirstName, firstName);
    }
    
    [Fact]
    public void PropertyAccessorShouldBeSuccessfullyConstructedWhenConversionIsNecessary()
    {
        var firstNameAccessor = GetCompiledPropertyAccessorInvocationResult<Person, object>(nameof(Person.FirstName));

        var personRandomizer = new PersonRandomizer();
        var person = personRandomizer.PrepareRandomValue();

        var firstName = firstNameAccessor(person);
        Assert.Equal(person.FirstName, firstName);
    }
    
    private static void AssertMemberInfoRetrieval<T, TValue>(Expression<Func<T, TValue>> selector, Type declaringType, string memberName)
    {
        var memberInfo = selector.GetMemberInfo();
        memberInfo.AssertSameMember(declaringType, memberName);
    }

    private static Func<T, TValue> GetCompiledPropertyAccessorInvocationResult<T, TValue>(string propertyName)
    {
        Assert.False(string.IsNullOrWhiteSpace(propertyName));

        var propertyInfo = typeof(T).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        
        var propertyAccessor = propertyInfo.ConstructPropertyAccessor<T, TValue>();
        Assert.NotNull(propertyAccessor);

        var compiledPropertyAccessor = propertyAccessor.Compile();
        Assert.NotNull(compiledPropertyAccessor);

        return compiledPropertyAccessor;
    }
}