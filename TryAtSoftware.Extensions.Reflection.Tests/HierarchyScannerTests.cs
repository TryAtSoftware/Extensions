namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using Xunit;

public class HierarchyScannerTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfAttributeShouldBeScannedForNullMember()
    {
        var hierarchyScanner = new HierarchyScanner();
        Assert.Throws<ArgumentNullException>(() => hierarchyScanner.ScanForAttribute<IdAttribute>(null!));
    }
    
    [Theory]
    [InlineData(typeof(ClassA), nameof(ClassA.Method), 1)]
    [InlineData(typeof(ClassB), nameof(ClassB.Method), 2)]
    [InlineData(typeof(ClassC), nameof(ClassC.Method), 1)]
    [InlineData(typeof(ClassD), nameof(ClassD.Method), 3)]
    [InlineData(typeof(ClassE), nameof(ClassE.Method), 4)]
    [InlineData(typeof(ClassF), nameof(ClassF.Method), 3)]
    public void MethodAttributeShouldBeSuccessfullyScanned(Type type, string methodName, int expectedId)
    {
        var decoratedMethod = type.GetMethod(methodName);
        Assert.NotNull(decoratedMethod);

        var hierarchyScanner = new HierarchyScanner();
        var scannedAttributes = hierarchyScanner.ScanForAttribute<IdAttribute>(decoratedMethod);

        var attribute = Assert.Single(scannedAttributes);
        Assert.NotNull(attribute);
        Assert.Equal(expectedId, attribute.Id);
    }

    [Theory]
    [InlineData(typeof(ClassA), nameof(ClassA.Property), 1)]
    [InlineData(typeof(ClassB), nameof(ClassB.Property), 2)]
    [InlineData(typeof(ClassC), nameof(ClassC.Property), 1)]
    [InlineData(typeof(ClassD), nameof(ClassD.Property), 3)]
    [InlineData(typeof(ClassE), nameof(ClassE.Property), 4)]
    [InlineData(typeof(ClassF), nameof(ClassF.Property), 3)]
    public void PropertyAttributeShouldBeSuccessfullyScanned(Type type, string propertyName, int expectedId)
    {
        var decoratedProperty = type.GetProperty(propertyName);
        Assert.NotNull(decoratedProperty);

        var hierarchyScanner = new HierarchyScanner();
        var scannedAttributes = hierarchyScanner.ScanForAttribute<IdAttribute>(decoratedProperty);

        var attribute = Assert.Single(scannedAttributes);
        Assert.NotNull(attribute);
        Assert.Equal(expectedId, attribute.Id);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
file class IdAttribute : Attribute
{
    public int Id { get; set; }
}

file class ClassA
{
    [Id(Id = 1)]
    public virtual string Property => throw new NotImplementedException();
    
    [Id(Id = 1)]
    public virtual void Method() => throw new NotImplementedException();
}

file class ClassB : ClassA
{
    [Id(Id = 2)]
    public override string Property => throw new NotImplementedException();

    [Id(Id = 2)]
    public override void Method() => throw new NotImplementedException();
}

file class ClassC : ClassA
{
    public override string Property => throw new NotImplementedException();

    public override void Method() => throw new NotImplementedException();
}

[Id(Id = 3)]
file class ClassD
{
    public string Property => throw new NotImplementedException();
    public void Method() => throw new NotImplementedException();
}

[Id(Id = 4)]
file class ClassE : ClassD
{
}

file class ClassF : ClassD
{
}
