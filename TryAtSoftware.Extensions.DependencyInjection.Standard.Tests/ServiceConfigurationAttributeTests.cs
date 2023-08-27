namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Tests;

using Microsoft.Extensions.DependencyInjection;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
using Xunit;

public class ServiceConfigurationAttributeTests
{
    [Theory, MemberData(nameof(GetServiceLifetimeData))]
    public void ServiceLifetimeShouldBeSetCorrectly(ServiceLifetime lifetime)
    {
        var attribute = new ServiceConfigurationAttribute { Lifetime = lifetime };
        Assert.Equal(lifetime, attribute.Lifetime);
    }

    public static IEnumerable<object[]> GetServiceLifetimeData() => Enum.GetValues<ServiceLifetime>().Select(x => new object[] { x });
}