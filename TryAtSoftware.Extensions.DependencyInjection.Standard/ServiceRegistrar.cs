namespace TryAtSoftware.Extensions.DependencyInjection.Standard;

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// An implementation of the <see cref="IServiceRegistrar"/> interface responsible for registering services into the built-in dependency injection container.
/// </summary>
public class ServiceRegistrar : IServiceRegistrar
{
    private readonly IServiceCollection _services;
    private readonly IHierarchyScanner _hierarchyScanner;
    private readonly Func<Type, Type>? _transformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRegistrar"/> class.
    /// </summary>
    /// <param name="serviceCollection">An <see cref="IServiceCollection"/> instance where the services will be automatically registered.</param>
    /// <param name="hierarchyScanner">An <see cref="IHierarchyScanner"/> instance used to scan for <see cref="ServiceConfigurationAttribute"/> and extract any additional configuration during the automatic registration of the services.</param>
    /// <param name="transformation">A function that should apply some transformation to the service types, e.g. resolve generic parameters, make a pointer type, etc. This is an optional parameter.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="serviceCollection"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="hierarchyScanner"/> is null.</exception>
    public ServiceRegistrar(IServiceCollection serviceCollection, IHierarchyScanner hierarchyScanner, Func<Type, Type>? transformation = null)
    {
        this._services = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        this._hierarchyScanner = hierarchyScanner ?? throw new ArgumentNullException(nameof(hierarchyScanner));
        this._transformation = transformation;
    }

    /// <inheritdoc />
    public void Register(Type type)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (!type.IsClass || type.IsAbstract) throw new InvalidOperationException("Only non-abstract classes can be registered into a dependency injection container.");

        var implementationType = this.Transform(type);
        var implementedInterfaces = implementationType.GetInterfaces().Select(this.Transform);

        var lifetime = this.ExtractLifetime(type);
        
        this.RegisterService(implementationType, implementationType, lifetime);
        foreach (var interfaceType in implementedInterfaces) this.RegisterService(interfaceType, implementationType, lifetime);
    }

    private void RegisterService(Type interfaceType, Type implementationType, ServiceLifetime lifetime)
    {
        var serviceDescriptor = new ServiceDescriptor(interfaceType, implementationType, lifetime);
        this._services.Add(serviceDescriptor);
    }

    private Type Transform(Type type)
    {
        if (this._transformation is null) return type;

        var transformedType = this._transformation(type);
        return transformedType ?? throw new InvalidOperationException("Type is null after applying transformation.");
    }

    private ServiceLifetime ExtractLifetime(MemberInfo type)
    {
        var lifetimeAttributes = this._hierarchyScanner.ScanForAttribute<ServiceConfigurationAttribute>(type);
        return lifetimeAttributes.LastOrDefault()?.Lifetime ?? ServiceLifetime.Scoped;
    }
}