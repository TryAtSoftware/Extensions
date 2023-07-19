[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=bugs)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)

[![Core validation](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml/badge.svg)](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml)

# About the project

`TryAtSoftware.Extensions.DependencyInjection` is a library containing extension methods and utility components that should simplify some common operations with dependency injection.

# About us

`Try At Software` is a software development company based in Bulgaria. We are mainly using `dotnet` technologies (`C#`, `ASP.NET Core`, `Entity Framework Core`, etc.) and our main idea is to provide a set of tools that can simplify the majority of work a developer does on a daily basis.

# Getting started

## Installing the package

In order to use this library, you need to install the corresponding NuGet package beforehand.
The simplest way to do this is to either use the `NuGet package manager`, or the `dotnet CLI`.

Using the `NuGet package manager` console within Visual Studio, you can install the package using the following command:

> Install-Package TryAtSoftware.Extensions.DependencyInjection

Or using the `dotnet CLI` from a terminal window:

> dotnet add package TryAtSoftware.Extensions.DependencyInjection

## Registering services

The `TryAtSoftware.Extensions.DependencyInjection` library **supports** automatic registration of services into a given dependency injection container.
This can be realized throughout the `AutoRegisterServices` extension method.
It will locate all classes decorated with the `AutomaticallyRegisteredService` attribute and register them as services using a concrete implementation of the [`IServiceRegistrar`](#iserviceregistrar) interface.

### Use cases

#### Register services from all assemblies

```C#
IServiceRegistrar serviceRegistrar = PrepareServiceRegistrar();
Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

allAssemblies.AutoRegisterServices(serviceRegistrar);
```
It is important to note that the `AppDomain.CurrentDomain.GetAssemblies()` invocation will return only those assemblies that are already loaded for the current domain.
If this cannot be guaranteed, the assemblies that contain classes that are expected to be registered automatically, should be loaded explicitly.

This problem can be solved easily if the assembly extension methods provided by `TryAtSoftware.Extensions.Reflection` are used.
For more information, you can refer to the [official documentation](https://github.com/TryAtSoftware/Extensions/blob/main/TryAtSoftware.Extensions.Reflection.md#assembly-extensions).

```C#
// It is recommended to use a `RestrictSearchFilter` in order to load only what is necessary.
var options = new LoadReferencedAssembliesOptions { RestrictSearchFilter = x => x.FullName.StartsWith("My.Awesome.Prefix")};
Assembly.GetExecutingAssembly().LoadReferencedAssemblies(options);

IServiceRegistrar serviceRegistrar = PrepareServiceRegistrar();
Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
allAssemblies.AutoRegisterServices(serviceRegistrar);
```

#### Resolve generic parameters

TODO

### `IServiceRegistrar`

This is an interface defining the structure of a component responsible for registering services into a dependency injection container.

The officially supported libraries providing implementations of this interface will be listed at the home page of the [`TryAtSoftware.Extensions`](https://github.com/TryAtSoftware/Extensions) repository.