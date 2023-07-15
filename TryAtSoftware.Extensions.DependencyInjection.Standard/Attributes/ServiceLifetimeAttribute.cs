namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;

using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ServiceLifetimeAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public ServiceLifetimeAttribute(ServiceLifetime lifetime)
    {
        this.Lifetime = lifetime;
    }
}