namespace TryAtSoftware.Extensions.DependencyInjection.Tests;

using System.Reflection;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using TryAtSoftware.Extensions.DependencyInjection.Attributes;
using TryAtSoftware.Extensions.DependencyInjection.Interfaces;
using TryAtSoftware.Extensions.DependencyInjection.Options;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public class DependencyInjectionTests
{
    [Theory, MemberData(nameof(GetRegisterServiceOptionsData))]
    public void AutomaticRegistrationOfServicesShouldBeExecutedSuccessfully(RegisterServiceOptions? options)
    {
        var assemblies = new Assembly[RandomizationHelper.RandomInteger(2, 10)];
        var services = new Type[assemblies.Length][];

        var total = 0;
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
            total += servicesCount;
        }

        var registrar = Substitute.For<IServiceRegistrar>();
        assemblies.AutoRegisterServices(registrar, options);

        for (var i = 0; i < services.Length; i++)
        {
            for (var j = 0; j < services[i].Length; j++)
                registrar.Received(1).Register(Arg.Is<Type>(x => ReferenceEquals(x, services[i][j])), options);
        }

        Assert.Equal(total, registrar.ReceivedCalls().Count());
    }

    [Fact]
    public void AutoRegisterServicesShouldThrowIfNullRegistrarIsProvided()
    {
        var assemblies = new [] { MockAssembly(Array.Empty<Type>()) };
        Assert.Throws<ArgumentNullException>(() => assemblies.AutoRegisterServices(serviceRegistrar: null!));
    }

    public static IEnumerable<object?[]> GetRegisterServiceOptionsData()
    {
        yield return new object?[] { null };
        yield return new object?[] { new RegisterServiceOptions() };
    }

    private static Assembly MockAssembly(Type[] exportedTypes)
    {
        var assemblyMock = Substitute.For<Assembly>();
        assemblyMock.GetExportedTypes().Returns(exportedTypes);
        return assemblyMock;
    }
    
    private static Type MockType(bool isAutomaticallyRegistered)
    {
        var typeMock = Substitute.For<Type>();
        typeMock.IsDefined(typeof(AutomaticallyRegisteredServiceAttribute), Arg.Any<bool>()).Returns(isAutomaticallyRegistered);
        
        return typeMock;
    }
}