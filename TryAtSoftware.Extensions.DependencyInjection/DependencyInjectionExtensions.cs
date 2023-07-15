namespace TryAtSoftware.Extensions.DependencyInjection;

using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.DependencyInjection.Attributes;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;

public static class DependencyInjectionExtensions
{
    public static void AutoRegisterServices(this IEnumerable<Assembly> assemblies, IServiceRegistrar serviceRegistrar)
    {
        if (serviceRegistrar is null) throw new ArgumentNullException(nameof(serviceRegistrar));

        foreach (var assembly in assemblies.OrEmptyIfNull().IgnoreNullValues())
        {
            var types = assembly.GetExportedTypes().Where(x => x.IsDefined(typeof(AutomaticallyRegisteredServiceAttribute), inherit: true));
            foreach (var type in types) serviceRegistrar.Register(type);
        }
    }
}