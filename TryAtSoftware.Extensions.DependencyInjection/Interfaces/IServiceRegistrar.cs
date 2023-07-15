namespace TryAtSoftware.Extensions.DependencyInjection.Interfaces;

/// <summary>
/// An interface defining the structure of a component responsible for registering services into a DI container.
/// </summary>
public interface IServiceRegistrar
{
    /// <summary>
    /// Use this method to register a service of the given <paramref name="type"/> into a DI container.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the service.</param>
    void Register(Type type);
}