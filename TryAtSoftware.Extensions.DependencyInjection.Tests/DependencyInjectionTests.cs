namespace TryAtSoftware.Extensions.DependencyInjection.Tests;

using System.Reflection;
using Moq;
using TryAtSoftware.Extensions.DependencyInjection.Attributes;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class DependencyInjectionTests
{
    [Fact]
    public void AutomaticRegistrationOfServicesShouldBeExecutedSuccessfully()
    {
        var assemblies = new Assembly[RandomizationHelper.RandomInteger(2, 10)];
        var services = new Type[assemblies.Length][];

        for (var i = 0; i < assemblies.Length; i++)
        {
            var servicesCount = RandomizationHelper.RandomInteger(0, 100); 
            var otherClassesCount = RandomizationHelper.RandomInteger(0, 100);
            services[i] = new Type[servicesCount];

            var exportedTypes = new Type[servicesCount + otherClassesCount];

            for (var j = 0; j < servicesCount; j++) services[i][j] = MockType(isAutomaticallyRegistered: true);
            for (var j = 0; j < servicesCount; j++) exportedTypes[j] = services[i][j];
            for (var j = 0; j < otherClassesCount; j++) exportedTypes[servicesCount + j] = MockType(isAutomaticallyRegistered: false);

            assemblies[i] = MockAssembly(exportedTypes);
        }

        var registrarMock = new Mock<IServiceRegistrar>();
        registrarMock.Setup(x => x.Register(It.IsAny<Type>()));

        var registrarInstance = registrarMock.Object;
        assemblies.AutoRegisterServices(registrarInstance);

        for (var i = 0; i < services.Length; i++)
        {
            for (var j = 0; j < services[i].Length; j++)
            {
                var serviceType = services[i][j];
                registrarMock.Verify(x => x.Register(serviceType), Times.Once);
            }
        }
        
        registrarMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void AutoRegisterServicesShouldThrowIfNullRegistrarIsProvided()
    {
        var assemblies = new [] { MockAssembly(Array.Empty<Type>()) };
        Assert.Throws<ArgumentNullException>(() => assemblies.AutoRegisterServices(serviceRegistrar: null!));
    }

    private static Assembly MockAssembly(Type[] exportedTypes)
    {
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetExportedTypes()).Returns(exportedTypes);
        return assemblyMock.Object;
    }
    
    private static Type MockType(bool isAutomaticallyRegistered)
    {
        var typeMock = new Mock<Type>();
        typeMock.Setup(x => x.IsDefined(typeof(AutomaticallyRegisteredServiceAttribute), It.IsAny<bool>())).Returns(isAutomaticallyRegistered);
        return typeMock.Object;
    }
}