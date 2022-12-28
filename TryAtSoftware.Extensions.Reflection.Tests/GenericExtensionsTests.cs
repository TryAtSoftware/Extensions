namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Collections.Generic;
using TryAtSoftware.Extensions.Reflection.Tests.Models;
using Xunit;

public class GenericExtensionsTests
{
    [Fact]
    public void ExtractGenericParametersSetupShouldHandleNullType()
    {
        var typesMap = PrepareTypesMap();
        Assert.Throws<ArgumentNullException>(() => ((Type)null!).ExtractGenericParametersSetup(typesMap));
    }
    
    [Fact]
    public void ExtractGenericParametersSetupShouldHandleNullTypesMap()
    {
        var type = typeof(GenericType<,>);
        Assert.Throws<ArgumentNullException>(() => type.ExtractGenericParametersSetup(null!));
    }

    [Fact]
    public void ExtractGenericParametersShouldHandleMissingParameterAttributes()
    {
        var type = typeof(GenericTypeWithNoDecoratedParameters<,>);
        var typesMap = PrepareTypesMap();

        Assert.Throws<InvalidOperationException>(() => type.ExtractGenericParametersSetup(typesMap));
    }

    [Fact]
    public void ExtractGenericParametersShouldHandleMultipleParameterAttributes()
    {
        var type = typeof(GenericTypeWithMultipleDecoratedParameters<,>);
        var typesMap = PrepareTypesMap();

        Assert.Throws<InvalidOperationException>(() => type.ExtractGenericParametersSetup(typesMap));
    }

    [Fact]
    public void ExtractGenericParametersShouldHandleMissingEntryInTheTypesMap()
    {
        var type = typeof(GenericType<,>);
        var typesMap = PrepareTypesMap();
        typesMap.Remove(typeof(GenericParameter2Attribute));

        Assert.Throws<InvalidOperationException>(() => type.ExtractGenericParametersSetup(typesMap));
    }

    [Fact]
    public void ExtractGenericParametersSetupShouldWorkCorrectlyWithNonGenericTypes()
    {
        var typesMap = PrepareTypesMap();
        var genericParametersSetup = typeof(Person).ExtractGenericParametersSetup(typesMap);
        
        Assert.NotNull(genericParametersSetup);
        Assert.Empty(genericParametersSetup);
    }
    
    [Fact]
    public void ExtractGenericParametersSetupShouldWorkCorrectlyWithGenericTypes()
    {
        var typesMap = PrepareTypesMap();
        var genericParametersSetup = typeof(GenericType<,>).ExtractGenericParametersSetup(typesMap);
        
        Assert.NotNull(genericParametersSetup);
        Assert.NotEmpty(genericParametersSetup);
        
        Assert.True(genericParametersSetup.ContainsKey("T1"));
        Assert.Equal(genericParametersSetup["T1"], typesMap[typeof(GenericParameter1Attribute)]);
        
        Assert.True(genericParametersSetup.ContainsKey("T2"));
        Assert.Equal(genericParametersSetup["T2"], typesMap[typeof(GenericParameter2Attribute)]);
    }
    
    [Fact]
    public void MakeGenericTypeShouldHandleNullType()
    {
        var genericParametersSetup = typeof(GenericType<,>).ExtractGenericParametersSetup(PrepareTypesMap());
        Assert.Throws<ArgumentNullException>(() => ((Type)null!).MakeGenericType(genericParametersSetup));
    }
    
    [Fact]
    public void MakeGenericTypeShouldHandleNullGenericParametersSetup()
    {
        var type = typeof(GenericType<,>);
        Assert.Throws<ArgumentNullException>(() => type.MakeGenericType(genericParametersSetup: null!));
    }
    
    [Fact]
    public void MakeGenericTypeShouldWorkCorrectlyWithNonGenericTypes()
    {
        var genericParametersSetup = typeof(Person).ExtractGenericParametersSetup(PrepareTypesMap());
        var builtGenericPersonType = typeof(Person).MakeGenericType(genericParametersSetup);
        
        Assert.NotNull(builtGenericPersonType);
        Assert.Equal(typeof(Person), builtGenericPersonType);
    }

    [Fact]
    public void MakeGenericTypeShouldHandleIncompleteGenericParametersSetup()
    {
        var genericParametersSetup = typeof(GenericType<,>).ExtractGenericParametersSetup(PrepareTypesMap());
        genericParametersSetup.Remove("T1");

        var builtGenericType = typeof(GenericType<,>).MakeGenericType(genericParametersSetup);
        Assert.NotNull(builtGenericType);
        Assert.True(builtGenericType.IsConstructedGenericType);

        var genericParameters = builtGenericType.GetGenericArguments();
        Assert.True(genericParameters[0].IsGenericParameter);
        Assert.True(genericParameters[0].IsGenericTypeParameter);
        
        Assert.Equal(genericParameters[1], genericParametersSetup["T2"]);
        Assert.False(genericParameters[1].IsGenericParameter);
        Assert.False(genericParameters[1].IsGenericTypeParameter);
    }
    
    [Fact]
    public void MakeGenericTypeShouldWorkCorrectlyWithGenericTypes()
    {
        var genericParametersSetup = typeof(GenericType<,>).ExtractGenericParametersSetup(PrepareTypesMap());
        var builtGenericType = typeof(GenericType<,>).MakeGenericType(genericParametersSetup);

        Assert.NotNull(builtGenericType);
        Assert.True(builtGenericType.IsConstructedGenericType);

        var genericParameters = builtGenericType.GetGenericArguments();
        foreach (var parameter in genericParameters)
        {
            Assert.False(parameter.IsGenericParameter);
            Assert.False(parameter.IsGenericTypeParameter);
        }
        
        Assert.Equal(genericParameters[0], genericParametersSetup["T1"]);
        Assert.Equal(genericParameters[1], genericParametersSetup["T2"]);
    }

    [Fact]
    public void MakeGenericTypeShouldWorkCorrectlyWithComplexTypes()
    {
        var type = typeof(ComplexGenericType<,>);
        var predecessorType = Assert.Single(type.GetInterfaces());
        
        var genericParametersSetup = typeof(ComplexGenericType<,>).ExtractGenericParametersSetup(PrepareTypesMap());
        var builtGenericPredecessorType = predecessorType.MakeGenericType(genericParametersSetup);
        Assert.NotNull(builtGenericPredecessorType);

        var expectedGenericKvpType = typeof(KeyValuePair<,>).MakeGenericType(genericParametersSetup["T1"], genericParametersSetup["T2"]);
        var expectedGenericListType = typeof(List<>).MakeGenericType(expectedGenericKvpType);
        var expectedGenericPredecessorType = typeof(IGenericExtensionsTestHelperInterface<>).MakeGenericType(expectedGenericListType);
        Assert.Equal(expectedGenericPredecessorType, builtGenericPredecessorType);
    }

    private static IDictionary<Type, Type> PrepareTypesMap() => new Dictionary<Type, Type> { { typeof(GenericParameter1Attribute), typeof(int) }, { typeof(GenericParameter2Attribute), typeof(string) } };

    [AttributeUsage(AttributeTargets.GenericParameter)]
    private sealed class GenericParameter1Attribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.GenericParameter)]
    private sealed class GenericParameter2Attribute : Attribute
    {
    }
    
#pragma warning disable S2326
    private class GenericType<[GenericParameter1] T1, [GenericParameter2] T2>
    {
    }
    
    private sealed class GenericTypeWithNoDecoratedParameters<T1, T2>
    {
    }
    
    private sealed class GenericTypeWithMultipleDecoratedParameters<[GenericParameter1, GenericParameter2] T1, T2>
    {
    }
    
    private sealed class ComplexGenericType<[GenericParameter1] T1, [GenericParameter2] T2> : IGenericExtensionsTestHelperInterface<List<KeyValuePair<T1, T2>>>
    {
    }
    
    private interface IGenericExtensionsTestHelperInterface<T>
    {
    }
#pragma warning restore S2326
}