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
    [InlineData(typeof(TransientService), ServiceLifetime.Transient)]
    [InlineData(typeof(ExplicitlyScopedService), ServiceLifetime.Scoped)]
    [InlineData(typeof(ImplicitlyScopedService), ServiceLifetime.Scoped)]
    [InlineData(typeof(SingletonService), ServiceLifetime.Singleton)]
    public void ServicesShouldBeSuccessfullyRegistered(Type implementationType, ServiceLifetime expectedLifetime)
    {
        var (serviceCollection, _, registrar) = InstantiateRegistrar();
        registrar.Register(implementationType);

        var interfaces = implementationType.GetInterfaces();
        Assert.Equal(interfaces.Length + 1, serviceCollection.Count);

        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, implementationType, expectedLifetime);

        Assert.Contains(implementationType, registeredServiceTypes);
        foreach (var i in interfaces) Assert.Contains(i, registeredServiceTypes);
    }
    
    [Fact]
    public void GenericServicesShouldBeSuccessfullyRegistered()
    {
        var implementationType = typeof(GenericService<>);
        
        var keyType = typeof(int);
        var registrationOptions = new RegisterServiceOptions { GenericTypesMap = new Dictionary<Type, Type> { [typeof(KeyTypeAttribute)] = keyType } };
        var (serviceCollection, _, registrar) = InstantiateRegistrar();

        registrar.Register(implementationType, registrationOptions);

        var interfaces = implementationType.GetInterfaces();
        Assert.Equal(interfaces.Length + 1, serviceCollection.Count);

        var genericImplementationType = implementationType.MakeGenericType(keyType);
        var genericInterfaceType = typeof(IGenericInterface<>).MakeGenericType(keyType); 
        var registeredServiceTypes = AssertSuccessfulRegistration(serviceCollection, genericImplementationType, ServiceLifetime.Scoped);

        Assert.Contains(genericImplementationType, registeredServiceTypes);
        Assert.Contains(genericInterfaceType, registeredServiceTypes);
    }

    private static (IServiceCollection Services, IHierarchyScanner HierarchyScanner, IServiceRegistrar Registrar) InstantiateRegistrar()
    {
        var serviceCollection = new ServiceCollection();
        var hierarchyScanner = new HierarchyScanner();
        var registrar = new ServiceRegistrar(serviceCollection, hierarchyScanner);

        return (serviceCollection, hierarchyScanner, registrar);
    }

    private static HashSet<Type> AssertSuccessfulRegistration(IServiceCollection serviceCollection, Type implementationType, ServiceLifetime expectedLifetime)
    {
        var registeredServiceTypes = new HashSet<Type>();
        foreach (var descriptor in serviceCollection)
        {
            Assert.True(registeredServiceTypes.Add(descriptor.ServiceType), "There are at least two descriptors for the same service type.");

            Assert.Null(descriptor.ImplementationFactory);
            Assert.Null(descriptor.ImplementationInstance);
            Assert.Equal(implementationType, descriptor.ImplementationType);
            Assert.Equal(expectedLifetime, descriptor.Lifetime);
        }

        return registeredServiceTypes;
    }
    
    private interface IBaseInterface {}
    private interface IGenericInterface<TKey> {}
    private interface IImplementedInterface1 : IBaseInterface {}
    private interface IImplementedInterface2 : IBaseInterface {}
    private abstract class BaseService : IImplementedInterface1, IImplementedInterface2 {}

    private class Service : BaseService {}
    [ServiceConfiguration(Lifetime = ServiceLifetime.Transient)] private class TransientService : BaseService {}
    [ServiceConfiguration(Lifetime = ServiceLifetime.Scoped)] private class ExplicitlyScopedService : BaseService {}
    [ServiceConfiguration] private class ImplicitlyScopedService : BaseService {}
    [ServiceConfiguration(Lifetime = ServiceLifetime.Singleton)] private class SingletonService : BaseService {}
    
    [AttributeUsage(AttributeTargets.GenericParameter)] private class KeyTypeAttribute : Attribute {}
    private class GenericService<[KeyType] TKey> : IGenericInterface<TKey> {}
}