namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using Xunit;

public class HierarchyScannerTests
{
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
        var scannedAttributes = hierarchyScanner.Scan<IdAttribute>(decoratedMethod);

        var attribute = Assert.Single(scannedAttributes);
        Assert.NotNull(attribute);
        Assert.Equal(expectedId, attribute.Id);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
file class IdAttribute : Attribute
{
    public int Id { get; set; }
}

file class ClassA
{
    [Id(Id = 1)]
    public virtual void Method() => throw new NotImplementedException();
}

file class ClassB : ClassA
{
    [Id(Id = 2)]
    public override void Method() => throw new NotImplementedException();
}

file class ClassC : ClassA
{
    public override void Method() => throw new NotImplementedException();
}

[Id(Id = 3)]
file class ClassD
{
    public void Method() => throw new NotImplementedException();
}

[Id(Id = 4)]
file class ClassE : ClassD
{
}

file class ClassF : ClassD
{
}
