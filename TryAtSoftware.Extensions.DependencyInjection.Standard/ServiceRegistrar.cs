namespace TryAtSoftware.Extensions.DependencyInjection.Standard;

using System;
using System.Collections.Generic;
using System.Linq;
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

        var configurationAttributes = this._hierarchyScanner.ScanForAttribute<ServiceConfigurationAttribute>(type).ToArray();

        var (implementationType, implementedInterfaces) = BuildServiceTypes(type, configurationAttributes, options);

        this.RegisterService(implementationType, implementationType, configurationAttributes);
        foreach (var interfaceType in implementedInterfaces) this.RegisterService(interfaceType, implementationType, configurationAttributes);
    }

    private static (Type ImplementationType, IEnumerable<Type> ImplementedInterfaces) BuildServiceTypes(Type type, ServiceConfigurationAttribute[] configurationAttributes, RegisterServiceOptions? options)
    {
        var implementedInterfaces = type.GetInterfaces();
        if (!type.IsGenericType) return (type, implementedInterfaces);

        if (ShouldBeRegisteredAsOpenGeneric(configurationAttributes))
        {
            var genericArguments = type.GetGenericArguments();
            return (type, implementedInterfaces.Where(x => genericArguments.SequenceEqual(x.GenericTypeArguments)).Select(x => x.GetGenericTypeDefinition()));
        }

        var genericParametersSetup = ExtractGenericParametersSetup(type, options);
        return (ResolveGenericParameters(type, genericParametersSetup), implementedInterfaces.Select(x => ResolveGenericParameters(x, genericParametersSetup)));
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

    private void RegisterService(Type interfaceType, Type implementationType, ServiceConfigurationAttribute[] configurationAttributes)
    {
        var lifetime = ExtractLifetime(configurationAttributes);

        ServiceDescriptor? serviceDescriptor = null;
#if NET8_0_OR_GREATER
        var key = ExtractKey(configurationAttributes);
        if (!string.IsNullOrWhiteSpace(key)) serviceDescriptor = new ServiceDescriptor(interfaceType, key, implementationType, lifetime);
#endif

        serviceDescriptor ??= new ServiceDescriptor(interfaceType, implementationType, lifetime);
        this._services.Add(serviceDescriptor);
    }

    private static ServiceLifetime ExtractLifetime(ServiceConfigurationAttribute[] configurationAttributes)
    {
        for (var i = configurationAttributes.Length - 1; i >= 0; i--)
        {
            if (configurationAttributes[i].LifetimeIsSet)
                return configurationAttributes[i].Lifetime;
        }

        return ServiceLifetime.Scoped;
    }

    private static bool ShouldBeRegisteredAsOpenGeneric(ServiceConfigurationAttribute[] configurationAttributes)
    {
        for (var i = configurationAttributes.Length - 1; i >= 0; i--)
        {
            if (configurationAttributes[i].OpenGenericIsSet)
                return configurationAttributes[i].OpenGeneric;
        }

        return false;
    }

#if NET8_0_OR_GREATER
    private static string? ExtractKey(ServiceConfigurationAttribute[] configurationAttributes)
    {
        for (var i = configurationAttributes.Length - 1; i >= 0; i--)
        {
            if (!string.IsNullOrWhiteSpace(configurationAttributes[i].Key))
                return configurationAttributes[i].Key;
        }

        return null;
    }
#endif
}