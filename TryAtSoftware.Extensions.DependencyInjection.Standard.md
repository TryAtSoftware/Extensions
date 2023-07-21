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

TODO

# Helpful Links

For a better understanding of the way the built-in dependency injection mechanisms work, you can refer to the [official documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).
Here are some more links pointing to the corresponding packages - [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions) and [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection).