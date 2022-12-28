namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Linq.Expressions;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Tests.Models;
using TryAtSoftware.Extensions.Reflection.Tests.Models.Specialized;
using TryAtSoftware.Extensions.Reflection.Tests.Randomization;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class ExpressionsExtensionsTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheGetMemberInfoMethod() => Assert.Throws<ArgumentNullException>(() => ((Expression<Func<Person, string>>)null!).GetMemberInfo());

    [Fact]
    public void MemberInfoShouldBeSuccessfullyRetrieved() => AssertMemberInfoRetrieval<Person, string?>(p => p.FirstName, typeof(Person), nameof(Person.FirstName));

    [Fact]
    public void MemberInfoShouldBeSuccessfullyRetrievedFromDerivedClasses() => AssertMemberInfoRetrieval<Student, string?>(p => p.FirstName, typeof(Person), nameof(Person.FirstName));

    [Fact]
    public void ExceptionShouldBeThrownIfMemberInfoCannotBeRetrievedSuccessfully()
    {
        Expression<Func<Person, string?>> expression = p => p.ToString();
        Assert.Throws<InvalidOperationException>(() => expression.GetMemberInfo());
    }

    [Fact]
    public void PropertyAccessorShouldBeSuccessfullyConstructed()
    {
        var firstNameAccessor = GetCompiledPropertyAccessor<Person, string>(nameof(Person.FirstName));

        var personRandomizer = new PersonRandomizer();
        var person = personRandomizer.PrepareRandomValue();

        var firstName = firstNameAccessor(person);
        Assert.Equal(person.FirstName, firstName);
    }

    [Fact]
    public void AccessorShouldBeSuccessfullyConstructedForInaccessibleProperties()
    {
        var inaccessiblePropertyAccessor = GetCompiledPropertyAccessor<ModelWithInaccessibleProperty, string>("InaccessibleProperty", BindingFlags.NonPublic);

        var inaccessiblePropertyValue = RandomizationHelper.GetRandomString();
        var testInstance = new ModelWithInaccessibleProperty { InaccessiblePropertySetter = inaccessiblePropertyValue };

        var inaccessibleValue = inaccessiblePropertyAccessor(testInstance);
        Assert.Equal(inaccessiblePropertyValue, inaccessibleValue);
    }

    [Fact]
    public void PropertyAccessorShouldBeSuccessfullyConstructedWhenConversionIsNecessary()
    {
        var firstNameAccessor = GetCompiledPropertyAccessor<Person, object>(nameof(Person.FirstName));

        var personRandomizer = new PersonRandomizer();
        var person = personRandomizer.PrepareRandomValue();

        var firstName = firstNameAccessor(person);
        Assert.Equal(person.FirstName, firstName);
    }

    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheConstructPropertyAccessorMethod() => Assert.Throws<ArgumentNullException>(() => ((PropertyInfo)null!).ConstructPropertyAccessor<Person, string>());

    [Fact]
    public void PropertyAccessorShouldNotBeConstructedIfTheReflectedTypeDoesNotCorrespondToTheProvidedGenericTypeParameter()
    {
        var firstNameProperty = typeof(Student).GetProperty(nameof(Student.FirstName));
        Assert.NotNull(firstNameProperty);

        Assert.Throws<InvalidOperationException>(() => firstNameProperty.ConstructPropertyAccessor<Person, string>());
    }

    [Fact]
    public void PropertyAccessorShouldNotBeConstructedIfItIsNotReadable()
    {
        var inaccessiblePropertySetter = typeof(ModelWithInaccessibleProperty).GetProperty(nameof(ModelWithInaccessibleProperty.InaccessiblePropertySetter));
        Assert.NotNull(inaccessiblePropertySetter);

        Assert.Throws<InvalidOperationException>(() => inaccessiblePropertySetter.ConstructPropertyAccessor<ModelWithInaccessibleProperty, string>());
    }

    [Fact]
    public void PropertySetterShouldBeConstructedSuccessfully()
    {
        var firstNameSetter = GetCompiledPropertySetter<Person, string>(nameof(Person.FirstName));

        var person = new Person();
        var randomString = RandomizationHelper.GetRandomString();

        firstNameSetter(person, randomString);
        Assert.Equal(randomString, person.FirstName);
    }

    [Fact]
    public void SetterShouldBeSuccessfullyConstructedForInaccessibleProperties()
    {
        var inaccessiblePropertySetter = GetCompiledPropertySetter<ModelWithInaccessibleProperty, string>("InaccessibleProperty", BindingFlags.NonPublic);

        var inaccessiblePropertyValue = RandomizationHelper.GetRandomString();
        var testInstance = new ModelWithInaccessibleProperty();

        inaccessiblePropertySetter(testInstance, inaccessiblePropertyValue);
        Assert.Equal(inaccessiblePropertyValue, testInstance.InaccessiblePropertyGetter);
    }

    [Fact]
    public void PropertySetterShouldBeConstructedSuccessfullyWhenConversionIsRequired()
    {
        var byteAgeSetter = GetCompiledPropertySetter<Person, byte>(nameof(Person.Age));
        var objectAgeSetter = GetCompiledPropertySetter<Person, object>(nameof(Person.Age));

        var person = new Person();
        
        var randomByteAge = (byte)RandomizationHelper.RandomInteger(byte.MinValue, byte.MaxValue);
        byteAgeSetter(person, randomByteAge);
        Assert.Equal(randomByteAge, person.Age);
        
        var randomInt16Age = (ushort)RandomizationHelper.RandomInteger(ushort.MinValue, ushort.MaxValue);
        objectAgeSetter(person, randomInt16Age);
        Assert.Equal(randomInt16Age, person.Age);
    }

    [Fact]
    public void PropertySetterShouldNotBeConstructedIfTheReflectedTypeDoesNotCorrespondToTheProvidedGenericTypeParameter()
    {
        var firstNameProperty = typeof(Student).GetProperty(nameof(Student.FirstName));
        Assert.NotNull(firstNameProperty);

        Assert.Throws<InvalidOperationException>(() => firstNameProperty.ConstructPropertySetter<Person, string>());
    }

    [Fact]
    public void PropertySetterShouldNotBeConstructedIfItIsReadOnly()
    {
        var inaccessiblePropertyGetter = typeof(ModelWithInaccessibleProperty).GetProperty(nameof(ModelWithInaccessibleProperty.InaccessiblePropertyGetter));
        Assert.NotNull(inaccessiblePropertyGetter);

        Assert.Throws<InvalidOperationException>(() => inaccessiblePropertyGetter.ConstructPropertySetter<ModelWithInaccessibleProperty, string>());
    }

    [Fact]
    public void ExceptionShouldBeThrownIfNullExpressionIsSentToTheConstructPropertySetterMethod() => Assert.Throws<ArgumentNullException>(() => ((PropertyInfo)null!).ConstructPropertySetter<Person, string>());

    private static void AssertMemberInfoRetrieval<T, TValue>(Expression<Func<T, TValue>> selector, Type declaringType, string memberName)
    {
        var memberInfo = selector.GetMemberInfo();
        memberInfo.AssertSameMember(declaringType, memberName);
    }

    private static Func<T, TValue> GetCompiledPropertyAccessor<T, TValue>(string propertyName, BindingFlags additionalBindingFlags = 0)
    {
        Assert.False(string.IsNullOrWhiteSpace(propertyName));

        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | additionalBindingFlags);
        Assert.NotNull(propertyInfo);

        var propertyAccessor = propertyInfo.ConstructPropertyAccessor<T, TValue>();
        Assert.NotNull(propertyAccessor);

        var compiledPropertyAccessor = propertyAccessor.Compile();
        Assert.NotNull(compiledPropertyAccessor);

        return compiledPropertyAccessor;
    }

    private static Action<T, TValue> GetCompiledPropertySetter<T, TValue>(string propertyName, BindingFlags additionalBindingFlags = 0)
    {
        Assert.False(string.IsNullOrWhiteSpace(propertyName));

        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | additionalBindingFlags);
        Assert.NotNull(propertyInfo);

        var propertySetter = propertyInfo.ConstructPropertySetter<T, TValue>();
        Assert.NotNull(propertySetter);

        var compiledPropertySetter = propertySetter.Compile();
        Assert.NotNull(compiledPropertySetter);

        return compiledPropertySetter;
    }
}