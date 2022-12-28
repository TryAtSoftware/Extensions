namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Collections.Generic;
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
        var genericParametersSetup = typeof(NonGenericType).ExtractGenericParametersSetup(typesMap);
        
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

    private static IDictionary<Type, Type> PrepareTypesMap() => new Dictionary<Type, Type> { { typeof(GenericParameter1Attribute), typeof(int) }, { typeof(GenericParameter2Attribute), typeof(string) } };

    [AttributeUsage(AttributeTargets.GenericParameter)]
    private sealed class GenericParameter1Attribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.GenericParameter)]
    private sealed class GenericParameter2Attribute : Attribute
    {
    }
    
    private sealed class NonGenericType
    {
    }
    
#pragma warning disable S2326
    private sealed class GenericType<[GenericParameter1] T1, [GenericParameter2] T2>
    {
    }
    
    private sealed class GenericTypeWithNoDecoratedParameters<T1, T2>
    {
    }
    
    private sealed class GenericTypeWithMultipleDecoratedParameters<[GenericParameter1, GenericParameter2] T1, T2>
    {
    }
#pragma warning restore S2326
}