[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=bugs)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)

[![Core validation](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml/badge.svg)](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml)

# About the project

`TryAtSoftware.Extensions.DependencyInjection.Standard` is a library that extends `TryAtSoftware.Extensions.DependencyInjection` and operates with the built-in dependency injection mechanisms.

# About us

`Try At Software` is a software development company based in Bulgaria. We are mainly using `dotnet` technologies (`C#`, `ASP.NET Core`, `Entity Framework Core`, etc.) and our main idea is to provide a set of tools that can simplify the majority of work a developer does on a daily basis.

# Getting started

## Installing the package

In order to use this library, you need to install the corresponding NuGet package beforehand.
The simplest way to do this is to either use the `NuGet package manager`, or the `dotnet CLI`.

Using the `NuGet package manager` console within Visual Studio, you can install the package using the following command:

> Install-Package TryAtSoftware.Extensions.DependencyInjection.Standard

Or using the `dotnet CLI` from a terminal window:

> dotnet add package TryAtSoftware.Extensions.DependencyInjection.Standard

## Registering services

Basic understanding of the concepts introduced by [`TryAtSoftware.Extensions.DependencyInjection`](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.DependencyInjection.md#registering-services) is required.
This package implements the presented ideas for the built-in dependency injection mechanisms.

### Service configuration

This package extends the configuration mechanisms introduced by `TryAtSoftware.Extensions.DependencyInjection` and goes a step further by defining its own set of configuration options modeling the parameters (or behavior) applied to the registration process when operating with the built-in dependency injection container.

One of the most important characteristics for services registered in the built-in dependency injection container is their **lifetime**.
So it looks like this parameter should be easily configurable when the corresponding services are automatically registered.

In the context of `TryAtSoftware.Extensions.DependencyInjection.Standard`, this can be achieved by decorating the service with the `ServiceConfiguration` attribute.
It accepts a single required parameter - the `ServiceLifetime`.

> As noted before, this package does not alter the configuration mechanisms introduced by `TryAtSoftware.Extensions.DependencyInjection` - it extends them.
> Because of this, it is required to decorate your services with both the `AutomaticallyRegisteredService` and `ServiceConfiguration` attributes if additional configurations are required.

```C#
[AutomaticallyRegisteredService, ServiceConfiguration(ServiceLifetime.Transient)]
public class EmailSender : IEmailSender
{
    // Here goes the implementation of the email sender... 
}
```

If a service should be registered automatically, but there are no explicit configurations, it will be registered as **scoped** service by default.

```C#
[AutomaticallyRegisteredService] // There are no explicit configurations => the lifetime of this service will be scoped.
public class EmailSender : IEmailSender
{
    // Here goes the implementation of the email sender... 
}
```

### `ServiceRegistrar`

This class implements the [`IServiceRegistrar`](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.DependencyInjection.md#iserviceregistrar) interface.
Its only constructor accepts two parameters:
- An `IServiceCollection` instance where the services will be registered.
- An [`IHierarchyScanner`](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.Reflection.md#ihierarchyscanner) instance used to scan for the `ServiceConfiguration` attribute described above.
You can use this parameter if you need to customize the way configuration attributes are discovered.

> This `IServiceRegistrar` implementation realizes all generally applicable [use cases](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.DependencyInjection.md#use-cases).

The registration process for any service involves the following steps:
1. Resolve generic parameters.
2. Discover additional configurations _(using the hierarchy scanner)_.
3. Register the service _(within the service collection)_.
   - At least one service descriptor will **always** be registered. It will have its `ServiceType` and `ImplementationType` set to the type of the automatically registered service.
   - For each interface the service implements, another service descriptor will be registered as well. In this case, its `ServiceType` will be set to the type of the implemented interface.

```C#
IServiceCollection serviceCollection = new ServiceCollection();
IHierarchyScanner hierarchyScanner = new HierarchyScanner();
IServiceRegistrar registrar = new ServiceRegistrar(serviceCollection, hierarchyScanner);

Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
allAssemblies.AutoRegisterServices(registrar);
```

# Helpful Links

For a better understanding of the ideas implemented by this package, you can refer to the [official documentation](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.DependencyInjection.md) of `TryAtSoftware.Extensions.DependencyInjection`.

For a better understanding of the way the built-in dependency injection mechanisms work, you can refer to the [official documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).
Here are some more links pointing to the corresponding packages - [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions) and [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection).