namespace TryAtSoftware.Extensions.DependencyInjection.Options;

using System;
using System.Collections.Generic;

/// <summary>
/// A class representing the options that can be used to refine the process of registering services into a dependency injection container.
/// </summary>
public class RegisterServiceOptions
{
    /// <summary>
    /// Gets or sets the generic types map that should be used to resolve generic dependencies.
    /// </summary>
    public IDictionary<Type, Type>? GenericTypesMap { get; set; }
}