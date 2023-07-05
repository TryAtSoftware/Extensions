namespace TryAtSoftware.Extensions.Reflection.Tests.Options;

using System;
using System.Reflection;
using Moq;
using TryAtSoftware.Extensions.Reflection.Interfaces;
using TryAtSoftware.Extensions.Reflection.Options;
using Xunit;

public class LoadReferencedAssembliesOptionsTests
{
    [Fact]
    public void OptionsShouldHaveProperDefaultValuesUponInitialization()
    {
        var options = new LoadReferencedAssembliesOptions();
        Assert.Null(options.RestrictSearchFilter);

        Assert.NotNull(options.Loader);
        Assert.IsType<AssemblyLoader>(options.Loader);
    }

    [Fact]
    public void RestrictSearchFilterShouldBeSuccessfullySet()
    {
        // ReSharper disable ConvertToLocalFunction
        Func<AssemblyName, bool> filter = _ => true;
        // ReSharper restore ConvertToLocalFunction

        var options = new LoadReferencedAssembliesOptions { RestrictSearchFilter = filter };
        Assert.Same(filter, options.RestrictSearchFilter);
    }

    [Fact]
    public void LoaderShouldBeSuccessfullySet()
    {
        var loaderMock = new Mock<IAssemblyLoader>();
        var loader = loaderMock.Object;

        var options = new LoadReferencedAssembliesOptions { Loader = loader };
        Assert.Same(loader, options.Loader);
    }

    [Fact]
    public void ExceptionShouldBeThrownIfNullLoaderIsSet()
    {
        var options = new LoadReferencedAssembliesOptions();
        Assert.Throws<ArgumentNullException>(() => options.Loader = null!);
    }
}