namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Reflection;
using Xunit;

public class AssemblyExtensionsTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfNullAssemblyIsPassedToTheLoadReferencedAssembliesMethod() => Assert.Throws<ArgumentNullException>(() => ((Assembly)null!).LoadReferencedAssemblies());
}