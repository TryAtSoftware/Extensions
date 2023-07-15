namespace TryAtSoftware.Extensions.DependencyInjection.Standard;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public abstract class BaseServiceRegistrar : IServiceRegistrar
{
    protected BaseServiceRegistrar(IServiceCollection services, IHierarchyScanner hierarchyScanner, Func<Type, Type>? transformation)
    {
        this.Services = services ?? throw new ArgumentNullException(nameof(services));
        this.HierarchyScanner = hierarchyScanner ?? throw new ArgumentNullException(nameof(hierarchyScanner));
        this.Transformation = transformation;
    }

    public IServiceCollection Services { get; }
    public IHierarchyScanner HierarchyScanner { get; }
    public Func<Type, Type>? Transformation { get; }


    public void Register(Type type)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (!type.IsClass || type.IsAbstract) throw new InvalidOperationException("Only non-abstract classes can be registered into a DI container.");

        var implementationType = this.Transform(type);
        var implementedInterfaces = implementationType.GetInterfaces().Select(this.Transform);

        var lifetime = this.ExtractLifetime(type);
        foreach (var interfaceType in implementedInterfaces)
        {
            var serviceDescriptor = new ServiceDescriptor(interfaceType, implementationType, lifetime);
            this.Services.Add(serviceDescriptor);
        }
    }

    private Type Transform(Type type)
    {
        if (this.Transformation is null) return type;

        var transformedType = this.Transformation(type);
        return transformedType ?? throw new InvalidOperationException("Type is null after applying transformation.");
    }

    private ServiceLifetime ExtractLifetime(MemberInfo type)
    {
        var lifetimeAttributes = this.HierarchyScanner.ScanForAttribute<ServiceLifetimeAttribute>(type);
        return lifetimeAttributes.LastOrDefault()?.Lifetime ?? ServiceLifetime.Scoped;
    }
}