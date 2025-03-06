namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Tests;

using Microsoft.Extensions.DependencyInjection;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
#if NET8_0_OR_GREATER
using TryAtSoftware.Randomizer.Core.Helpers;
#endif
using Xunit;

public class ServiceConfigurationAttributeTests
{
    [Theory, MemberData(nameof(GetServiceLifetimeData))]
    public void ServiceLifetimeShouldBeSetCorrectly(ServiceLifetime lifetime)
    {
        var attribute = new ServiceConfigurationAttribute { Lifetime = lifetime };
        Assert.Equal(lifetime, attribute.Lifetime);
        Assert.True(attribute.LifetimeIsSet);
    }

    [Fact]
    public void LifetimeIsSetShouldReturnFalseWhenLifetimeIsNotSet()
    {
        var attribute = new ServiceConfigurationAttribute();
        Assert.False(attribute.LifetimeIsSet);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void ServiceKeyShouldHaveCorrectDefaultValue()
    {
        var attribute = new ServiceConfigurationAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void ServiceKeyShouldBeSetCorrectly()
    {
        var key = RandomizationHelper.GetRandomString();
        var attribute = new ServiceConfigurationAttribute { Key = key };

        Assert.Equal(key, attribute.Key);
    }

    [Fact]
    public void ServiceKeyShouldBeSetToNullCorrectly()
    {
        var attribute = new ServiceConfigurationAttribute { Key = null };
        Assert.Null(attribute.Key);
    }
#endif

    public static TheoryData<ServiceLifetime> GetServiceLifetimeData()
    {
        var serviceLifetimeValues = Enum.GetValues<ServiceLifetime>();
        return new TheoryData<ServiceLifetime>(serviceLifetimeValues);
    }
}