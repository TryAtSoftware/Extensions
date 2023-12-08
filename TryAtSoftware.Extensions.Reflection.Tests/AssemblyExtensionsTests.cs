namespace TryAtSoftware.Extensions.Reflection.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
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
        var assemblyLoaderMock = Substitute.For<IAssemblyLoader>();

        var rootAssembly = PopulateHierarchyOfAssemblies(firstLevel, secondLevel, assemblyLoaderMock);

        var options = new LoadReferencedAssembliesOptions { Loader = assemblyLoaderMock };
        rootAssembly.LoadReferencedAssemblies(options);

        var allAssemblyNames = new HashSet<AssemblyName>();
        for (var i = 0; i < firstLevel.Length; i++)
        {
            VerifyLoadInvocation(assemblyLoaderMock, firstLevel[i]);
            allAssemblyNames.Add(firstLevel[i].GetName());

            for (var j = 0; j < secondLevel[i].Length; j++)
            {
                VerifyLoadInvocation(assemblyLoaderMock, secondLevel[i][j]);
                allAssemblyNames.Add(secondLevel[i][j].GetName());
            }
        }

        assemblyLoaderMock.Received(Quantity.None()).Load(Arg.Is<AssemblyName>(x => !allAssemblyNames.Contains(x)));
    }

    [Fact]
    public void ReferencedAssembliesShouldBeLoadedCorrectlyWithAppliedFilter()
    {
        var firstLevel = new Assembly[RandomizationHelper.RandomInteger(3, 10)];
        var secondLevel = new Assembly[firstLevel.Length][];
        var assemblyLoaderMock = Substitute.For<IAssemblyLoader>();

        var rootAssembly = PopulateHierarchyOfAssemblies(firstLevel, secondLevel, assemblyLoaderMock);

        var randomBlockIndex = RandomizationHelper.RandomInteger(0, firstLevel.Length);
        var randomBlockName = firstLevel[randomBlockIndex].GetName();
        var options = new LoadReferencedAssembliesOptions { Loader = assemblyLoaderMock, RestrictSearchFilter = x => x != randomBlockName };
        rootAssembly.LoadReferencedAssemblies(options);

        var allAssemblyNames = new HashSet<AssemblyName>();
        for (var i = 0; i < firstLevel.Length; i++)
        {
            if (i == randomBlockIndex) continue;

            VerifyLoadInvocation(assemblyLoaderMock, firstLevel[i]);
            allAssemblyNames.Add(firstLevel[i].GetName());

            for (var j = 0; j < secondLevel[i].Length; j++)
            {
                VerifyLoadInvocation(assemblyLoaderMock, secondLevel[i][j]);
                allAssemblyNames.Add(secondLevel[i][j].GetName());
            }
        }

        assemblyLoaderMock.Received(Quantity.None()).Load(Arg.Is<AssemblyName>(x => !allAssemblyNames.Contains(x)));
    }

    private static Assembly PopulateHierarchyOfAssemblies(Assembly[] firstLevel, Assembly[][] secondLevel, IAssemblyLoader assemblyLoaderMock)
    {
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

        return rootAssembly;
    }

    private static Assembly PrepareAssemblyMock(string name, Assembly[] referencedAssemblies)
    {
        Assert.False(string.IsNullOrWhiteSpace(name));
        Assert.NotNull(referencedAssemblies);

        var mock = Substitute.For<Assembly>();

        var assemblyName = new AssemblyName { Name = name };
        mock.GetName().Returns(assemblyName);
        mock.GetReferencedAssemblies().Returns(_ => referencedAssemblies.Select(x => x.GetName()).ToArray());

        return mock;
    }

    private static void SetupLoadInvocation(IAssemblyLoader loaderMock, Assembly assembly)
    {
        Assert.NotNull(loaderMock);
        Assert.NotNull(assembly);

        loaderMock.Load(assembly.GetName()).Returns(assembly);
    }

    private static void VerifyLoadInvocation(IAssemblyLoader loaderMock, Assembly assembly)
    {
        Assert.NotNull(loaderMock);
        Assert.NotNull(assembly);

        loaderMock.Received(Quantity.Exactly(1)).Load(assembly.GetName());
    }
}