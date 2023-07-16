namespace TryAtSoftware.Extensions.DependencyInjection.Standard;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Extensions.DependencyInjection.Options;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
using TryAtSoftware.Extensions.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// An implementation of the <see cref="IServiceRegistrar"/> interface responsible for registering services into the built-in dependency injection container.
/// </summary>
public class ServiceRegistrar : IServiceRegistrar
{
    private readonly IServiceCollection _services;
    private readonly IHierarchyScanner _hierarchyScanner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRegistrar"/> class.
    /// </summary>
    /// <param name="serviceCollection">An <see cref="IServiceCollection"/> instance where the services will be automatically registered.</param>
    /// <param name="hierarchyScanner">An <see cref="IHierarchyScanner"/> instance used to scan for <see cref="ServiceConfigurationAttribute"/> and extract any additional configuration during the automatic registration of the services.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="serviceCollection"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="hierarchyScanner"/> is null.</exception>
    public ServiceRegistrar(IServiceCollection serviceCollection, IHierarchyScanner hierarchyScanner)
    {
        this._services = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        this._hierarchyScanner = hierarchyScanner ?? throw new ArgumentNullException(nameof(hierarchyScanner));
    }

    /// <inheritdoc />
    public void Register(Type type, RegisterServiceOptions? options = null)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (!type.IsClass || type.IsAbstract) throw new InvalidOperationException("Only non-abstract classes can be registered into a dependency injection container.");

        var genericParametersSetup = ExtractGenericParametersSetup(type, options);
        var implementationType = ResolveGenericParameters(type, genericParametersSetup);
        var implementedInterfaces = implementationType.GetInterfaces().Select(x => ResolveGenericParameters(x, genericParametersSetup));

        var lifetime = this.ExtractLifetime(type);
        
        this.RegisterService(implementationType, implementationType, lifetime);
        foreach (var interfaceType in implementedInterfaces) this.RegisterService(interfaceType, implementationType, lifetime);
    }

    private static IDictionary<string, Type>? ExtractGenericParametersSetup(Type serviceType, RegisterServiceOptions? options)
    {
        if (options?.GenericTypesMap is null) return null;
        return serviceType.ExtractGenericParametersSetup(options.GenericTypesMap);
    }

    private static Type ResolveGenericParameters(Type type, IDictionary<string, Type>? genericParametersSetup)
    {
        if (genericParametersSetup is null) return type;
        return type.MakeGenericType(genericParametersSetup);
    }

    private void RegisterService(Type interfaceType, Type implementationType, ServiceLifetime lifetime)
    {
        var serviceDescriptor = new ServiceDescriptor(interfaceType, implementationType, lifetime);
        this._services.Add(serviceDescriptor);
    }

    private ServiceLifetime ExtractLifetime(MemberInfo type)
    {
        var lifetimeAttributes = this._hierarchyScanner.ScanForAttribute<ServiceConfigurationAttribute>(type);
        return lifetimeAttributes.LastOrDefault()?.Lifetime ?? ServiceLifetime.Scoped;
    }
}