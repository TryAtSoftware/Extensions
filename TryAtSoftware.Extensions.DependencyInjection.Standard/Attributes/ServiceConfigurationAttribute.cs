namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An attribute that should provide information about the configuration to use when automatically registering the decorated class within the built-in dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ServiceConfigurationAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the lifetime of the decorated service.
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
}