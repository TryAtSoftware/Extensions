namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Linq;
using System.Reflection;
using Moq;
using TryAtSoftware.Extensions.Reflection.Interfaces;
using TryAtSoftware.Extensions.Reflection.Options;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class AssemblyExtensionsTests
{
    [Fact]
    public void ExceptionShouldBeThrownIfNullAssemblyIsPassedToTheLoadReferencedAssembliesMethod() => Assert.Throws<ArgumentNullException>(() => ((Assembly)null!).LoadReferencedAssemblies());

    [Fact]
    public void ReferencedAssembliesShouldBeLoadedCorrectly()
    {
        var firstLevel = new Assembly[RandomizationHelper.RandomInteger(3, 10)];
        var secondLevel = new Assembly[firstLevel.Length][];

        var assemblyLoaderMock = new Mock<IAssemblyLoader>();
        for (var i = 0; i < firstLevel.Length; i++)
        {
            secondLevel[i] = new Assembly[RandomizationHelper.RandomInteger(2, 5)];
            firstLevel[i] = PrepareAssemblyMock($"FL{i + 1}", secondLevel[i]);
            SetupLoadInvocation(assemblyLoaderMock, firstLevel[i]);

            for (var j = 0; j < secondLevel[i].Length; j++)
            {
                secondLevel[i][j] = PrepareAssemblyMock($"SL{i + 1}.{j + 1}", Array.Empty<Assembly>());
                SetupLoadInvocation(assemblyLoaderMock, secondLevel[i][j]);
            }
        }

        var rootAssembly = PrepareAssemblyMock("Root", firstLevel);
        SetupLoadInvocation(assemblyLoaderMock, rootAssembly);

        var options = new LoadReferencedAssembliesOptions { Loader = assemblyLoaderMock.Object };
        rootAssembly.LoadReferencedAssemblies(options);

        for (var i = 0; i < firstLevel.Length; i++)
        {
            VerifyLoadInvocation(assemblyLoaderMock, firstLevel[i]);
            for (var j = 0; j < secondLevel[i].Length; j++) VerifyLoadInvocation(assemblyLoaderMock, secondLevel[i][j]);
        }
        assemblyLoaderMock.VerifyNoOtherCalls();
    }

    private static Assembly PrepareAssemblyMock(string name, Assembly[] referencedAssemblies)
    {
        Assert.False(string.IsNullOrWhiteSpace(name));
        Assert.NotNull(referencedAssemblies);
        
        var mock = new Mock<Assembly>();
        
        var assemblyName = new AssemblyName { Name = name };
        mock.Setup(x => x.GetName()).Returns(() => assemblyName);
        mock.Setup(x => x.GetReferencedAssemblies()).Returns(() => referencedAssemblies.Select(x => x.GetName()).ToArray());

        return mock.Object;
    }

    private static void SetupLoadInvocation(Mock<IAssemblyLoader> loaderMock, Assembly assembly)
    {
        Assert.NotNull(loaderMock);
        Assert.NotNull(assembly);

        loaderMock.Setup(x => x.Load(assembly.GetName())).Returns<AssemblyName>(_ => assembly);
    }

    private static void VerifyLoadInvocation(Mock<IAssemblyLoader> loaderMock, Assembly assembly)
    {
        Assert.NotNull(loaderMock);
        Assert.NotNull(assembly);
        
        loaderMock.Verify(x => x.Load(assembly.GetName()), Times.Once);
    }
}