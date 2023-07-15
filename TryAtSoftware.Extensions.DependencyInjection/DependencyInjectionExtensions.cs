namespace TryAtSoftware.Extensions.DependencyInjection;

using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.DependencyInjection.Attributes;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;

/// <summary>
/// A static class containing extension methods that are useful when working with dependency injection containers.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Use this method to locate all classes from the extended <paramref name="assemblies"/> decorated with the <see cref="AutomaticallyRegisteredServiceAttribute"/> and register them as services into a dependency injection container. 
    /// </summary>
    /// <param name="assemblies">The extended collection of <see cref="Assembly"/> instances.</param>
    /// <param name="serviceRegistrar">An <see cref="IServiceRegistrar"/> instance responsible for registering services into a dependency injection container.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="serviceRegistrar"/> is null.</exception>
    public static void AutoRegisterServices(this IEnumerable<Assembly> assemblies, IServiceRegistrar serviceRegistrar)
    {
        if (serviceRegistrar is null) throw new ArgumentNullException(nameof(serviceRegistrar));

        foreach (var assembly in assemblies.OrEmptyIfNull().IgnoreNullValues())
        {
            var types = assembly.GetExportedTypes().Where(x => x.IsDefined(typeof(AutomaticallyRegisteredServiceAttribute), inherit: Constants.RegistrationIsInheritable));
            foreach (var type in types) serviceRegistrar.Register(type);
        }
    }
}