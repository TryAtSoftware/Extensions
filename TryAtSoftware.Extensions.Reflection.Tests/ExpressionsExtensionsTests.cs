namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
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
    public void ExceptionShouldBeThrownIfNullExpressionIsPassedToTheGetMemberInfoMethod() => Assert.Throws<ArgumentNullException>(() => ((Expression<Func<Person, string>>)null!).GetMemberInfo());

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

    [Theory]
    [MemberData(nameof(GeneratePersonInstances))]
    public void PropertyAccessorShouldBeSuccessfullyConstructed(Person person)
    {
        var firstNameAccessor = GetCompiledPropertyAccessor<Person, string>(nameof(Person.FirstName));

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
    public void ExceptionShouldBeThrownIfNullIsPassedToTheConstructPropertyAccessorMethod() => Assert.Throws<ArgumentNullException>(() => ((PropertyInfo)null!).ConstructPropertyAccessor<Person, string>());

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

    [Theory]
    [MemberData(nameof(GeneratePersonInstances))]
    public void PropertySetterShouldBeConstructedSuccessfully(Person person)
    {
        var firstNameSetter = GetCompiledPropertySetter<Person, string>(nameof(Person.FirstName));

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
    public void ExceptionShouldBeThrownIfNullIsPassedToTheConstructPropertySetterMethod() => Assert.Throws<ArgumentNullException>(() => ((PropertyInfo)null!).ConstructPropertySetter<Person, string>());

    [Theory]
    [MemberData(nameof(GenerateObjectInitializationParameters))]
    public void ConstructObjectInitializerShouldWorkCorrectly(string text, int number, char? symbol)
    {
        var constructorsBinder = new MembersBinder<ModelWithConstructors>(
            x => x.MemberType == MemberTypes.Constructor,
            x => PrepareConstructorName(Assert.IsAssignableFrom<ConstructorInfo>(x)),
            BindingFlags.Public | BindingFlags.Instance);
        Assert.Equal(4, constructorsBinder.MemberInfos.Count);

        var predefinedParameterValues = new Dictionary<Type, object?> { { typeof(string), text }, { typeof(int), number }, { typeof(char), symbol } };

        var constructorId = 1;
        foreach (var (_, member) in constructorsBinder.MemberInfos.OrderByDescending(x => x.Key))
        {
            var constructorInfo = Assert.IsAssignableFrom<ConstructorInfo>(member);
            AssertCorrectObjectInitialization(constructorInfo, constructorId++, predefinedParameterValues);
        }
    }

    [Fact]
    public void ConstructObjectInitializerShouldWorkCorrectlyForInaccessibleConstructors()
    {
        var constructor = typeof(ModelWithConstructors).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, new[] { typeof(int), typeof(string), typeof(int), typeof(char) });
        Assert.NotNull(constructor);

        var constructorId = RandomizationHelper.RandomInteger(10, 101);
        var text = RandomizationHelper.GetRandomString();
        var number = RandomizationHelper.RandomInteger(1, 1001);
        var symbol = (char)('a' + RandomizationHelper.RandomInteger(0, 26));

        var newInstanceInitializerExpression = constructor.ConstructObjectInitializer<ModelWithConstructors>();
        var newInstanceInitializer = newInstanceInitializerExpression.Compile();

        var instance = newInstanceInitializer(new object[] { constructorId, text, number, symbol });
        Assert.NotNull(instance);

        Assert.Equal(constructorId, instance.UsedConstructor);
        Assert.Equal(text, instance.Text);
        Assert.Equal(number, instance.Number);
        Assert.Equal(symbol, instance.Symbol);
    }

    [Fact]
    public void ObjectInitializerShouldThrowExceptionIfLessParametersAreProvided()
    {
        var constructor = typeof(ModelWithConstructors).GetConstructor(new[] { typeof(string), typeof(int), typeof(char) });
        Assert.NotNull(constructor);
        
        var newInstanceInitializerExpression = constructor.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true);
        var newInstanceInitializer = newInstanceInitializerExpression.Compile();

        Assert.Throws<InvalidOperationException>(() => newInstanceInitializer(Array.Empty<object?>()));
    }

    [Fact]
    public void ObjectInitializerShouldThrowExceptionIfMoreParametersAreProvided()
    {
        var constructor = typeof(ModelWithConstructors).GetConstructor(Array.Empty<Type>());
        Assert.NotNull(constructor);
        
        var newInstanceInitializerExpression = constructor.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true);
        var newInstanceInitializer = newInstanceInitializerExpression.Compile();

        Assert.Throws<InvalidOperationException>(() => newInstanceInitializer(new object?[] { 15 }));
    }

    [Fact]
    public void ObjectInitializerShouldNotBeConstructedIfTheReflectedTypeDoesNotCorrespondToTheProvidedGenericTypeParameter()
    {
        var personConstructor = typeof(Person).GetConstructor(Array.Empty<Type>());
        Assert.NotNull(personConstructor);
        Assert.Throws<InvalidOperationException>(() => personConstructor.ConstructObjectInitializer<ModelWithConstructors>());
    }

    [Fact]
    public void ObjectInitializerShouldNotBeConstructedForAbstractTypes()
    {
        var abstractModelConstructor = typeof(AbstractModel).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Array.Empty<Type>());
        Assert.NotNull(abstractModelConstructor);
        Assert.Throws<InvalidOperationException>(() => abstractModelConstructor.ConstructObjectInitializer<AbstractModel>());
    }

    [Fact]
    public void ExceptionShouldBeThrownIfNullIsPassedToTheConstructObjectInitializerMethod() => Assert.Throws<ArgumentNullException>(() => ((ConstructorInfo)null!).ConstructObjectInitializer<ModelWithConstructors>());

    public static IEnumerable<object[]> GeneratePersonInstances()
    {
        var personRandomizer = new PersonRandomizer();
        var studentRandomizer = new StudentRandomizer();

        yield return new object[] { personRandomizer.PrepareRandomValue() };
        yield return new object[] { studentRandomizer.PrepareRandomValue() };
    }

    public static IEnumerable<object?[]> GenerateObjectInitializationParameters()
    {
        yield return new object?[] { RandomizationHelper.GetRandomString(), RandomizationHelper.RandomInteger(1, 101), null };
        yield return new object[] { RandomizationHelper.GetRandomString(), RandomizationHelper.RandomInteger(1, 101), (char)('a' + RandomizationHelper.RandomInteger(0, 26)) };
    }

    private static void AssertCorrectObjectInitialization(ConstructorInfo constructorInfo, int constructorId, Dictionary<Type, object?> predefinedParameterValues)
    {
        Assert.NotNull(constructorInfo);

        var parameters = constructorInfo.GetParameters();
        var constructorParameterTypes = new HashSet<Type>();
        var objectInitializationArguments = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            constructorParameterTypes.Add(parameters[i].ParameterType);
            predefinedParameterValues.TryGetValue(parameters[i].ParameterType, out objectInitializationArguments[i]);
        }

        var newInstanceInitializerExpression = constructorInfo.ConstructObjectInitializer<ModelWithConstructors>();
        var newInstanceInitializer = newInstanceInitializerExpression.Compile();

        var instance = newInstanceInitializer(objectInitializationArguments);
        Assert.NotNull(instance);

        AssertValue(x => x.Text, () => ModelWithConstructors.DefaultText);
        AssertValue(x => x.Number, () => ModelWithConstructors.DefaultNumber);
        AssertValue(x => x.Symbol, () => ModelWithConstructors.DefaultSymbol);
        Assert.Equal(constructorId, instance.UsedConstructor);

        void AssertValue<T>(Func<ModelWithConstructors, T> valueSelector, Func<T> defaultValueSelector)
        {
            var instanceValue = valueSelector(instance);
            if (constructorParameterTypes.Contains(typeof(T)) && predefinedParameterValues.TryGetValue(typeof(T), out var predefinedValue) && predefinedValue is not null) Assert.Equal(predefinedValue, instanceValue);
            else Assert.Equal(defaultValueSelector(), instanceValue);
        }
    }

    private static string PrepareConstructorName(ConstructorInfo constructorInfo)
    {
        Assert.NotNull(constructorInfo);

        var parameterTypeNames = constructorInfo.GetParameters().Select(p => TypeNames.Get(p.ParameterType));
        return $"Constructor - {string.Join(", ", parameterTypeNames)}";
    }

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