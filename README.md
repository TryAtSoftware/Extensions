[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=bugs)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)

[![Core validation](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml/badge.svg)](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml)

# About the project

`TryAtSoftware.Extensions` is a repository containing many libraries with extension methods and utility components one can use to reduce repetitive code or simplify common tasks.

# About us

`Try At Software` is a software development company based in Bulgaria. We are mainly using `dotnet` technologies (`C#`, `ASP.NET Core`, `Entity Framework Core`, etc.) and our main idea is to provide a set of tools that can simplify the majority of work a developer does on a daily basis.

# TryAtSoftware.Extensions.Collection

This is a library containing extension methods that should simplify some common operations with collections.

### `OrEmptyIfNull`

This is an extension method that will return an empty enumerable if the extended one was null.
The main use case is to prevent unnecessary exceptions whenever a `null` enumerable should not be treated differently than an `empty` enumerable.

Examples of **incorrect** code:

```C#
IEnumerable<int> numbers = /* initialization... */;

// 1. Iterating an `IEnumerable<T>` instance
if (numbers != null)
{
    foreach (int num in numbers) { /* Do something */ }
}

// 2. Passing an `IEnumerable<T>` instance to other methods
string text;
if (numbers != null) text = string.Empty;
else text = string.Join(", ", numbers);
```

Examples of **correct** code:

```C#
IEnumerable<int> numbers = /* initialization... */;

// 1. Iterating an `IEnumerable<T>` instance
foreach (int num in numbers.OrEmptyIfNull()) { /* Do something */ }

// 2. Passing an `IEnumerable<T>` instance to other methods
string text = string.Join(", ", numbers.OrEmptyIfNull());
```

### `IgnoreNullValues`

This is an extension method that will return a new enumerable containing all values from the extended one that are not `null` in the same order.
The main use case is to reduce the amount of conditions when iterating a collection of elements.
Examples of **incorrect** code:

```C#
IEnumerable<string> words = /* initialization... */;

if (words != null)
{
    foreach (string w in words) 
    {
        if (w == null) continue;
 
        /* Do something */
    }
}
```

Examples of **correct** code:

```C#
IEnumerable<string> words = /* initialization... */;

foreach (string w in words.OrEmptyIfNull().IgnoreNullValues()) 
{
    // Do something
}
```

### `IgnoreDefaultValues`

This is an extension method that will return a new enumerable containing all values from the extended one that do not equal the default one in the same order.
The main use case is to reduce the amount of conditions when iterating a collection of elements.
Examples of **incorrect** code:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;

if (identifiers != null)
{
    foreach (Guid id in identifiers) 
    {
        if (id == Guid.Empty) continue;
 
        /* Do something */
    }
}
```

Examples of **correct** code:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;

foreach (Guid id in identifiers.OrEmptyIfNull().IgnoreDefaultValues()) 
{
    // Do something
}
```

### `SafeWhere`

### `ConcatenateWith`


# TryAtSoftware.Extensions.Reflection
