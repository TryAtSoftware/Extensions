namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An attribute that should provide information about the configuration to use when automatically registering the decorated class within the built-in dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ServiceConfigurationAttribute : Attribute
{
    private ServiceLifetime _lifetime;
    private bool _openGeneric;
    
    /// <summary>
    /// Gets a value indicating whether a value is set to the <see cref="Lifetime"/> property.
    /// </summary>
    internal bool LifetimeIsSet { get; private set; }

    /// <summary>
    /// Gets or sets the lifetime of the decorated service.
    /// </summary>
    public ServiceLifetime Lifetime
    {
        get => this._lifetime;
        set
        {
            this.LifetimeIsSet = true;
            this._lifetime = value;
        }
    }
    
    /// <summary>
    /// Gets a value indicating whether a value is set to the <see cref="OpenGeneric"/> property.
    /// </summary>
    internal bool OpenGenericIsSet { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the decorated class should be registered as an open generic type within the standard dependency injection container.
    /// </summary>
    public bool OpenGeneric
    {
        get => this._openGeneric;
        set
        {
            this.OpenGenericIsSet = true;
            this._openGeneric = value;
        }
    }

#if NET8_0_OR_GREATER
    public string? Key { get; set; }
#endif
}