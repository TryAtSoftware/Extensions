namespace TryAtSoftware.Extensions.DependencyInjection.Standard.Tests;

using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Extensions.DependencyInjection.Options;
using TryAtSoftware.Extensions.DependencyInjection.Standard.Attributes;
using TryAtSoftware.Extensions.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;
using Xunit;

public class ServiceRegistrarTests
{
    [Fact]
    public void ServiceRegistrarShouldNotBeInstantiatedSuccessfullyIfNullServiceCollectionIsProvided()
    {
        var hierarchyScanner = Substitute.For<IHierarchyScanner>();
        Assert.Throws<ArgumentNullException>(() => new ServiceRegistrar(null!, hierarchyScanner));
    }

    [Fact]
    public void ServiceRegistrarShouldNotBeInstantiatedSuccessfullyIfNullHierarchyScannerIsProvided()
    {
        var serviceCollection = Substitute.For<IServiceCollection>();
        Assert.Throws<ArgumentNullException>(() => new ServiceRegistrar(serviceCollection, null!));
    }

    [Fact]
    public void ExceptionShouldBeThrownIfNullIsProvidedToTheRegisterMethod()
    {
        var (_, _, registrar) = InstantiateRegistrar();
        Assert.Throws<ArgumentNullException>(() => registrar.Register(null!));
    }

    [Fact]
    public void ExceptionShouldBeThrownIfInterfaceIsProvidedToTheRegisterMethod()
    {
        var (_, _, registrar) = InstantiateRegistrar();
        Assert.Throws<InvalidOperationException>(() => registrar.Register(typeof(IBaseInterface)));
    }

    [Fact]
    public void ExceptionShouldBeThrownIfAbstractClassIsProvidedToTheRegisterMethod()
    {
        var (_, _, registrar) = InstantiateRegistrar();
        Assert.Throws<InvalidOperationException>(() => registrar.Register(typeof(BaseService)));
    }

    [Theory]
    [InlineData(typeof(Service), ServiceLifetime.Scoped)]
#if NET8_0_OR_GREATER
    [InlineData(typeof(KeyedService), ServiceLifetime.Scoped, "service_1")]
#endif
    [InlineData(typeof(TransientService), ServiceLifetime.Transient)]
    [InlineData(typeof(ExplicitlyScopedService), ServiceLifetime.Scoped)]
    [InlineData(typeof(ImplicitlyScopedService), ServiceLifetime.Scoped)]
    [InlineData(typeof(SingletonService), ServiceLifetime.Singleton)]
#if NET8_0_OR_GREATER
    public void ServicesShouldBeSuccessfullyRegistered(Type implementationType, ServiceLifetime expectedLifetime, object? expectedKey = null)
#else
    public void ServicesShouldBeSuccessfullyRegistered(Type implementationType, ServiceLifetime expectedLifetime)
#endif
    {
        var (serviceCollection, _, registrar) = InstantiateRegistrar();
        registrar.Register(implementationType);

        var interfaces = implementationType.GetInterfaces();
        Assert.Equal(interfaces.Length + 1, serviceCollection.Count);

#if NET8_0_OR_GREATER
        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, implementationType, expectedLifetime, expectedKey);
#else
        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, implementationType, expectedLifetime);
#endif

        Assert.Contains(implementationType, registeredServiceTypes);
        foreach (var i in interfaces) Assert.Contains(i, registeredServiceTypes);
    }

    /// <remarks>
    /// For the <paramref name="genericTypesSetup"/> parameter,
    /// attribute types are followed by the actual generic parameter type.
    /// </remarks>
    [Theory]
    [InlineData(typeof(GenericService<>), new[] { typeof(IGenericInterface<>) }, new[] { typeof(KeyTypeAttribute), typeof(int) })]
    [InlineData(typeof(GenericService<,>), new[] { typeof(IGenericInterface<,>) }, new[] { typeof(KeyTypeAttribute), typeof(int), typeof(ValueTypeAttribute), typeof(string) })]
    public void GenericServicesShouldBeSuccessfullyRegistered(Type implementationType, Type[] serviceTypes, Type[] genericTypesSetup)
    {
        var genericTypesMatrix = genericTypesSetup.Chunk(2).ToArray();
        var registrationOptions = new RegisterServiceOptions { GenericTypesMap = genericTypesMatrix.ToDictionary(x => x[0], x => x[1]) };
        var (serviceCollection, _, registrar) = InstantiateRegistrar();

        registrar.Register(implementationType, registrationOptions);

        Assert.Equal(serviceTypes.Length + 1, serviceCollection.Count);

        var genericTypeArguments = genericTypesMatrix.Select(x => x[1]).ToArray();
        var genericImplementationType = implementationType.MakeGenericType(genericTypeArguments);
        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, genericImplementationType, ServiceLifetime.Scoped);

        Assert.Contains(genericImplementationType, registeredServiceTypes);

        foreach (var serviceType in serviceTypes)
        {
            var genericServiceType = serviceType.MakeGenericType(genericTypeArguments);
            Assert.Contains(genericServiceType, registeredServiceTypes);
        }
    }

    [Theory]
    [InlineData(typeof(OpenGenericService<>), new[] { typeof(IGenericInterface<>) })]
    [InlineData(typeof(OpenGenericService<,>), new[] { typeof(IGenericInterface<,>) })]
    public void OpenGenericServicesShouldBeSuccessfullyRegistered(Type implementationType, Type[] serviceTypes)
    {
        var (serviceCollection, _, registrar) = InstantiateRegistrar();
        registrar.Register(implementationType);

        Assert.Equal(serviceTypes.Length + 1, serviceCollection.Count);

        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, implementationType, ServiceLifetime.Scoped);

        Assert.Contains(implementationType, registeredServiceTypes);
        foreach (var interfaceType in serviceTypes) Assert.Contains(interfaceType, registeredServiceTypes);
    }

    private static (IServiceCollection Services, IHierarchyScanner HierarchyScanner, IServiceRegistrar Registrar) InstantiateRegistrar()
    {
        var serviceCollection = new ServiceCollection();
        var hierarchyScanner = new HierarchyScanner();
        var registrar = new ServiceRegistrar(serviceCollection, hierarchyScanner);

        return (serviceCollection, hierarchyScanner, registrar);
    }

#if NET8_0_OR_GREATER
    private static HashSet<Type> AssertSuccessfulRegistration(IServiceCollection serviceCollection, Type implementationType, ServiceLifetime expectedLifetime, object? expectedKey = null)
#else
    private static HashSet<Type> AssertSuccessfulRegistration(IServiceCollection serviceCollection, Type implementationType, ServiceLifetime expectedLifetime)
#endif
    {
        var registeredServiceTypes = new HashSet<Type>();
        foreach (var descriptor in serviceCollection)
        {
            Assert.True(registeredServiceTypes.Add(descriptor.ServiceType), "There are at least two descriptors for the same service type.");

            var executeStandardAssertions = true;

#if NET8_0_OR_GREATER
            if (expectedKey is not null)
            {
                Assert.Null(descriptor.KeyedImplementationFactory);
                Assert.Null(descriptor.KeyedImplementationInstance);
                Assert.Equal(implementationType, descriptor.KeyedImplementationType);

                Assert.True(descriptor.IsKeyedService);
                Assert.Equal(expectedKey, descriptor.ServiceKey);

                executeStandardAssertions = false;
            }
#endif

            if (executeStandardAssertions)
            {
                Assert.Null(descriptor.ImplementationFactory);
                Assert.Null(descriptor.ImplementationInstance);
                Assert.Equal(implementationType, descriptor.ImplementationType);
            }

            Assert.Equal(expectedLifetime, descriptor.Lifetime);
        }

        return registeredServiceTypes;
    }

    private interface IBaseInterface;
    private interface IGenericInterface<T>;
    private interface IGenericInterface<T1, T2>;
    private interface IImplementedInterface1 : IBaseInterface;
    private interface IImplementedInterface2 : IBaseInterface;
    private abstract class BaseService : IImplementedInterface1, IImplementedInterface2;

    private class Service : BaseService;
    [ServiceConfiguration(Lifetime = ServiceLifetime.Transient)] private class TransientService : BaseService;
    [ServiceConfiguration(Lifetime = ServiceLifetime.Scoped)] private class ExplicitlyScopedService : BaseService;
    [ServiceConfiguration] private class ImplicitlyScopedService : BaseService;
    [ServiceConfiguration(Lifetime = ServiceLifetime.Singleton)] private class SingletonService : BaseService;

    [AttributeUsage(AttributeTargets.GenericParameter)] private class KeyTypeAttribute : Attribute;
    [AttributeUsage(AttributeTargets.GenericParameter)] private class ValueTypeAttribute : Attribute;
    [ServiceConfiguration] private class GenericService<[KeyType] TKey> : IGenericInterface<TKey>;
    [ServiceConfiguration] private class GenericService<[KeyType] TKey, [ValueType] TValue> : IGenericInterface<TKey, TValue>;

    [ServiceConfiguration(OpenGeneric = true)] private class OpenGenericService<T> : IGenericInterface<T>, IGenericInterface<(T First, T Second)>;
    [ServiceConfiguration(OpenGeneric = true)] private class OpenGenericService<T1, T2> : IGenericInterface<T1, T2>, IGenericInterface<(T1 First, T2 Second)>;

#if NET8_0_OR_GREATER
    [ServiceConfiguration(Key = "service_1")] private class KeyedService : BaseService;
#endif
}