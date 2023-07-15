namespace TryAtSoftware.Extensions.DependencyInjection.Tests;

using TryAtSoftware.Extensions.DependencyInjection.Options;
using Xunit;

public class RegisterServiceOptionsTests
{
    [Fact]
    public void RegisterServiceOptionsShouldBeInstantiatedSuccessfully()
    {
        var options = new RegisterServiceOptions();
        Assert.Null(options.GenericTypesMap);
    }

    [Fact]
    public void GenericTypesMapShouldBeSetAndRetrievedSuccessfully()
    {
        var genericTypesMap = new Dictionary<Type, Type>();
        var options = new RegisterServiceOptions { GenericTypesMap = genericTypesMap };
        Assert.Same(genericTypesMap, options.GenericTypesMap);
    }
}