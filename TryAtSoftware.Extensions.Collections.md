[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=bugs)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)

[![Core validation](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml/badge.svg)](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml)

# About the project

`TryAtSoftware.Extensions.Collection` is a library containing extension methods that should simplify some common operations with collections.

# About us

`Try At Software` is a software development company based in Bulgaria. We are mainly using `dotnet` technologies (`C#`, `ASP.NET Core`, `Entity Framework Core`, etc.) and our main idea is to provide a set of tools that can simplify the majority of work a developer does on a daily basis.

# Getting started

## Installing the package

In order to use this library, you need to install the corresponding NuGet package beforehand.
The simplest way to do this is to either use the `NuGet package manager`, or the `dotnet CLI`.

Using the `NuGet package manager` console within Visual Studio, you can install the package using the following command:

> Install-Package TryAtSoftware.Extensions.Collections

Or using the `dotnet CLI` from a terminal window:

> dotnet add package TryAtSoftware.Extensions.Collections

## Data structures

### Bitmask

This is a specialized data structure designed for efficient manipulation of sequence of bits throughout bitwise operations.
The proposed implementation can be used to work with bitmasks of various length (especially with long bitmasks).

#### Initialization

The `Bitmask` constructor accepts two parameters - the **length** of the bitmask and a boolean indicating the initial state of all bits - whether they should be set or unset.

```C#
Bitmask bitmaskWithZeros = new Bitmask(8, initializeWithZeros: true); // 00000000
Bitmask bitmaskWithOnes = new Bitmask(8, initializeWithZeros: false); // 11111111
```

#### Manually setting bits

The `Set` method can be used to set the bit at a given position.
It accepts a single parameter - the **zero-based** position of the bit that should be set.

```C#
Bitmask bitmask = new Bitmask(8, initializeWithZeros: true); // 00000000
        
bitmask.Set(0); // 10000000
bitmask.Set(3); // 10010000

bitmask.Set(0); // The bit is already set - nothing will be changed.

// Invalid cases - position is out of range
bitmask.Set(-1); // Exception will be thrown!
bitmask.Set(8); // Exception will be thrown!
bitmask.Set(100); // Exception will be thrown!
```

#### Manually unsetting bits

The `Unset` method can be used to unset the bit at a given position.
It accepts a single parameter - the **zero-based** position of the bit that should be unset.

```C#
Bitmask bitmask = new Bitmask(8, initializeWithZeros: false); // 11111111
        
bitmask.Unset(1); // 10111111
bitmask.Unset(2); // 10011111

bitmask.Unset(2); // The bit is not set - nothing will be changed.

// Invalid cases - position is out of range
bitmask.Unset(-1); // Exception will be thrown!
bitmask.Unset(8); // Exception will be thrown!
bitmask.Unset(100); // Exception will be thrown!
```

#### Checking the status of a bit

The `IsSet` method can be used to check the status of a bit at a given position.
It accepts a single parameter - the **zero-based** position of the bit whose status should be checked.

```C#
Bitmask bitmask = new Bitmask(8, initializeWithZeros: true);
        
// Set the bits at even positions.
for (int i = 0; i < bitmask.Length; i += 2) bitmask.Set(i);

bool firstBitIsSet = bitmask.IsSet(0); // True
bool secondBitIsSet = bitmask.IsSet(1); // False

// Invalid cases - position is out of range
bitmask.IsSet(-1); // Exception will be thrown!
bitmask.IsSet(8); // Exception will be thrown!
bitmask.IsSet(100); // Exception will be thrown!
```

#### Executing bitwise operations

Bitwise operations can be executed by using the corresponding operators.
_They can come in handy whenever it is required to work over a group of bits collectively, rather than individually._
The produced result will be a `Bitmask` instance as well.

> For .NET 7 or above the `Bitmask` type implements the `IBitwiseOperators<Bitmask, Bitmask, Bitmask>` interface.

```C#
Bitmask bitmask1 = new Bitmask(8, initializeWithZeros: true);
Bitmask bitmask2 = new Bitmask(8, initializeWithZeros: true);

// Set the corresponding bits so the first bitmask looks like this: 11010100
bitmask1.Set(0); bitmask1.Set(1); bitmask1.Set(3); bitmask1.Set(5);

// Set the corresponding bits so the second bitmask looks like this: 01100101
bitmask2.Set(1); bitmask2.Set(2); bitmask2.Set(5); bitmask2.Set(7);

Bitmask andResult = bitmask1 & bitmask2; // 01000100
Bitmask orResult = bitmask1 | bitmask2; // 11110101
Bitmask xorResult = bitmask1 ^ bitmask2; // 10110001

Bitmask notResult1 = ~bitmask1; // 00101011
Bitmask notResult2 = ~bitmask2; // 10011010
```

#### Find the position of the least significant set bit

The `FindLeastSignificantSetBit` method can be used to find the position of the least significant **set** bit.
If there are no set bits, the returned value will be `-1`.

```C#
Bitmask bitmask = new Bitmask(8, initializeWithZeros: true);
        
// Set the corresponding bits so the second bitmask looks like this: 01001000
bitmask.Set(1); bitmask.Set(4);

var position1 = bitmask.FindLeastSignificantSetBit(); // 4

bitmask.Unset(4); // 01000000
var position2 = bitmask.FindLeastSignificantSetBit(); // 1

bitmask.Unset(1); // 00000000
var position3 = bitmask.FindLeastSignificantSetBit(); // -1
```

#### Find the position of the least significant unset bit

The `FindLeastSignificantUnsetBit` method can be used to find the position of the least significant **unset** bit.
If there are no unset bits, the returned value will be `-1`.

```C#
Bitmask bitmask = new Bitmask(8, initializeWithZeros: false);

// Unset the corresponding bits so the second bitmask looks like this: 11101101
bitmask.Unset(3); bitmask.Unset(6);

var position1 = bitmask.FindLeastSignificantUnsetBit(); // 6

bitmask.Set(6); // 11101111
var position2 = bitmask.FindLeastSignificantUnsetBit(); // 3

bitmask.Set(3); // 11111111
var position3 = bitmask.FindLeastSignificantUnsetBit(); // -1
```

## Collection extensions

### `OrEmptyIfNull`

This is an extension method that will return an empty enumerable if the extended one was null.
The main use case is to prevent unnecessary exceptions whenever a `null` enumerable should not be treated differently than an `empty` enumerable.

Let's look at this example:

```C#
IEnumerable<int> numbers = /* initialization... */;

// 1. Iterating an `IEnumerable<T>` instance
if (numbers != null)
{
    foreach (int num in numbers) { /* Do something */ }
}

// 2. Passing an `IEnumerable<T>` instance to other methods
string text;
if (numbers == null) text = string.Empty;
else text = string.Join(", ", numbers);
```

It can be **improved** like this:

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

Let's look at this example:

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

It can be **improved** like this:

```C#
IEnumerable<string> words = /* initialization... */;

foreach (string w in words.OrEmptyIfNull().IgnoreNullValues())
{
    // Do something
}
```

### `IgnoreNullOrWhitespaceValues`

This is an extension method that will return a new enumerable containing all string values from the extended one that are not null or empty and do not consist of whitespace characters only in the same order.
The main use case is to reduce the amount of conditions when iterating a collection of string elements.

Let's look at this example:

```C#
IEnumerable<string> words = /* initialization... */;

if (words != null)
{
    foreach (string w in words)
    {
        if (string.IsNullOrWhitespace(w)) continue;

        /* Do something */
    }
}
```

It can be **improved** like this:

```C#
IEnumerable<string> words = /* initialization... */;

foreach (string w in words.OrEmptyIfNull().IgnoreNullOrWhitespaceValues())
{
    // Do something
}
```

### `IgnoreDefaultValues`

This is an extension method that will return a new enumerable containing all values from the extended one that do not equal the default one in the same order.
The main use case is to reduce the amount of conditions when iterating a collection of elements.

Let's look at this example:

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

It can be **improved** like this:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;

foreach (Guid id in identifiers.OrEmptyIfNull().IgnoreDefaultValues())
{
    // Do something
}
```

### `SafeWhere`

This is an extension method that can be used to filter the elements of the extended `enumerable` safely in terms of the nullability of the `predicate`.

Let's look at this example:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;

if (identifiers != null)
{
    Predicate<Guid> predicate = /* initialization... */;
    if (predicate != null) identifiers = identifiers.Where(predicate);

    foreach (Guid id in identifiers)
    {
        /* Do something */
    }
}
```

It can be **improved** like this:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;
Predicate<Guid> predicate = /* initialization... */;

foreach (Guid id in identifiers.OrEmptyIfNull().SafeWhere(predicate))
{
    // Do something
}
```

### `ConcatenateWith`

This is an extension method that can be used to concatenate two collections safely in terms of their nullability.
Let's look at this example:

```C#
IEnumerable<int> a = /* initialization... */;
IEnumerable<int> b = /* initialization... */;

IEnumerable<int> concatenated;
if (a == null && b == null) concatenated = Enumerable.Empty<int>();
else if (a == null) concatenated = b;
else if (b == null) concatenated = a;
else
{
    List<int> tempConcatenated = new List<int>();
    foreach (int el in a) tempConcatenated.Add(el);
    foreach (int el in b) tempConcatenated.Add(el);

    concatenated = tempConcatenated;
}
```

It can be **improved** like this:

```C#
IEnumerable<int> a = /* initialization... */;
IEnumerable<int> b = /* initialization... */;

IEnumerable<int> concatenated = a.ConcatenateWith(b);
```

### `Union`

This extension method will produce the union of multiple `IEnumerable<T>` instances.

Example:

```C#
HashSet<int> a = new HashSet<int> { 1, 2, 3 };
HashSet<int> b = new HashSet<int> { 2, 3, 4 };
HashSet<int> c = new HashSet<int> { 3, 4, 5 };

HashSet<int>[] allSets = new [] { a, b, c };
HashSet<int> union = allSets.Union(); // This will contain: 1, 2, 3, 4, 5
```

### `AsReadOnlyCollection`

Use this method to easily construct an `IReadOnlyCollection<T>` instance from the extended collection.
This extension method is safe in terms of the nullability of the extended collection.

Let's look at this example:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;

IReadOnlyCollection<Guid> readOnlyCollection;
if (identifiers == null) readOnlyCollection = new List<Guid>().AsReadOnly();
else readOnlyCollection = identifiers.ToList().AsReadOnly();
```

It can be **improved** like this:

```C#
IEnumerable<Guid> identifiers = /* initialization... */;
IReadOnlyCollection<Guid> readOnlyCollection = identifiers.AsReadOnlyCollection();
```

## Dictionary extensions

### `OrEmptyIfNull`

This is an extension method that will return an empty dictionary if the extended one was `null`.
The main use case is to prevent unnecessary exceptions whenever a `null` dictionary should not be treated differently than an `empty` dictionary.

Let's look at this example:

```C#
IDictionary<int, int> numbersMap = /* initialization... */;

// 1. Iterating an `IDictionary<TKey, TValue>` instance
if (numbersMap != null)
{
    foreach (int (num, count) in numbers) { /* Do something */ }
}

// 2. Passing an `IDictionary<TKey, TValue>` instance to other methods
string text;
if (numbers == null) text = string.Empty;
else text = string.Join(", ", numbers.Select(x => $"{x.Key}: {x.Value}"));
```

It can be **improved** like this:

```C#
IDictionary<int, int> numbersMap = /* initialization... */;

// 1. Iterating an `IDictionary<TKey, TValue>` instance
foreach (int (num, count) in numbersMap.OrEmptyIfNull()) { /* Do something */ }

// 2. Passing an `IDictionary<TKey, TValue>` instance to other methods
string text = string.Join(", ", numbersMap.OrEmptyIfNull().Select(x => $"{x.Key}: {x.Value}"));
```

### `EnsureValue`

Use this method to ensure that a value within a dictionary exists for the requested key.
In order to use this extension method, the `TValue` type should be a type that exposes an empty constructor.
If the extended dictionary contains the requested key and a value associated with it, that value will be returned.
Else, a new instance of the `TValue` type will be created, saved within the dictionary for the requested key and returned by the method.

Let's look at this example:

```C#
IDictionary<string, HashSet<int>> wordsMap = /* initialization... */;

string key = "Hello, world!";
if (!wordsMap.ContainsKey(key)) wordsMap[key] = new HashSet<int>();
wordsMap[key].Add(1);
```

It can be **improved** like this:

```C#
IDictionary<string, HashSet<int>> wordsMap = /* initialization... */;

string key = "Hello, world!";
HashSet<int> set = wordsMap.EnsureValue(key);
set.Add(1);
```

### `MapSafely`

Use this method to form a mapping between the selected keys and values for each element in the extended collection.
This extension method is safe in terms of what happens whenever two elements have the same key - no exception is thrown and the original value remains unchanged.

```C#
IEnumerable<int> numbers = /* initialization... */;

// The following line may throw an exception if `numbers` contains the same number twice.
Dictionary<int, int> doubleValues = numbers.ToDictionary(x => x, x => x * 2);
```

It can be **improved** like this:

```C#
IEnumerable<int> numbers = /* initialization... */;
IDictionary<int, int> doubleValues = numbers.MapSafely(x => x, x => x * 2);
```

### `AsReadOnlyDictionary`

Use this method to easily construct an `IReadOnlyDictionary<TKey, TValue>` instance from the extended dictionary.
This extension method is safe in terms of the nullability of the extended dictionary.

Let's look at this example:

```C#
IDictionary<int, int> numbersMap = /* initialization... */;

IReadOnlyDictionary<int, int> readOnlyDictionary;
if (numbersMap == null) readOnlyDictionary = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());
else readOnlyDictionary = new ReadOnlyDictionary<int, int>(numbersMap);
```

It can be **improved** like this:

```C#
IDictionary<int, int> numbersMap = /* initialization... */;
IReadOnlyDictionary<int, int> readOnlyDictionary = numbersMap.AsReadOnlyDictionary();
```
