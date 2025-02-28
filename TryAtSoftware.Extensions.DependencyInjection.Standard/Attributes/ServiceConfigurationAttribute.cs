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

    /// <summary>
    /// Gets a value indicating whether or not a value is set to the <see cref="Lifetime"/> property.
    /// </summary>
    public bool LifetimeIsSet { get; private set; }

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

#if NET8_0_OR_GREATER
    private string[] _keys = [];

    public string? Key
    {
        get
        {
            if (this.Keys.Length != 1) return null;
            return this.Keys[0];
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value)) this.Keys = [];
            else this.Keys = [value];
        }
    }

    public string[] Keys
    {
        get => this._keys;
        set => this._keys = value ?? throw new ArgumentNullException(nameof(value));
    }
#endif
}