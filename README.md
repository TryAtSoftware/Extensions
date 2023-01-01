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

# _TryAtSoftware.Extensions.Collection_

This is a library containing extension methods that should simplify some common operations with collections.

## Collections

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
HashSet<int> intersection = allSets.SetUnion(); // This will contain: 1, 2, 3, 4, 5
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

## Dictionaries

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

# _TryAtSoftware.Extensions.Reflection_

This is a library containing extension methods and utility components that should simplify (and optimize) some common operations with reflection.

### `TypeNames`

This is an utility component that should construct and cache a beautified name for a type.
There are two ways to use it:

- Throughout the generic `TypeNames<T>` class and the `Value` property;
- Throughout the non-generic `TypeNames` class and the `Get(type)` method.

The main idea behind this component is to provide efficiently a meaningful and readable type name (including open generics).
Here is a comparison between the `type.ToString()` and `TypeNames.Get(type)`:

| type               | type.ToString()                                                                    | TypeNames.Get(type)  |
|--------------------|------------------------------------------------------------------------------------|----------------------|
| `Task<int>`        | System.Threading.Tasks.Task\`1\[System.Int32]                                      | Task\<Int32>         |
| `Task<List<long>>` | System.Collections.Generic.List\`1\[System.Threading.Tasks.Task\`1\[System.Int64]] | List\<Task\<Person>> |
| `Task<>`           | System.Threading.Tasks.Task\`1\[TResult]                                           | Task\<TResult>       |
| `List<>`           | System.Collections.Generic.List\`1\[T]                                             | List\<T>             |

### `IMembersBinder`

This is an interface defining the structure of an utility component exposing information about a specific subset of the members for a given type.
There are two classes implementing this interface - a non-generic `MembersBinder` and a generic `MembersBinder<T>`.
They accept the following parameters throughout their constructors:
- `isValid`: A filtering function that should determine whether or not a member should be included within the final result set. If no value is provided for this parameter, every member will be included. 
- `keySelector`: A function mapping each each member to unique key. If no value is provided to this parameter, the name of the member will be used (which means that in case of overrides or members with the same name there will be issues)
- `bindingFlags`: The binding flags used to control the member search throughout reflection.

Example:
```C#
IMembersBinder binder = new MembersBinder<TEntity>(IsValidMember, BindingFlags.Public | BindingFlags.Instance);

// Equivalent to:
// IMembersBinder = new MembersBinder(typeof(TEntity), IsValidMember, BindingFlags.Public | BindingFlags.Instance);

static bool IsValidMember(MemberInfo memberInfo)
    => memberInfo switch
    {
        PropertyInfo pi => pi.CanWrite,
        FieldInfo _ => true,
        _ => false
    };

// Now every discovered member for the `TEntity` type will be mapped against its name throughout the `binder.MemberInfos` dictionary.
```

### Expression extensions

#### `ConstructPropertyAccessor`

This is an extension method that should construct an expression for accessing the value of a specific property.
Usually, it is a good practice to minimize the reflection calls in code. One way of achieving this is throughout expressions (that are constructed and compiled only once for the lifetime of a program).
This expression method can be easily used with the `IMembersBinder` we described in the previous chapter.
Conversions are also handled, e.g. a common use case is to retrieve the values of all properties by boxing them as `object` instances - this can be achieved without any additional configurations as long as the required conversion can be executed.

Example:
```C#
IMembersBinder binder = new MembersBinder<TEntity>(x => x is PropertyInfo { CanRead: true}, BindingFlags.Public | BindingFlags.Instance);
List<Expression<Func<TEntity, object>>> valueAccessors = new List<Expression<Func<TEntity, object>>>();

foreach (var (_, memberInfo) in binder.MemberInfos)
{
    var propertyInfo = memberInfo as PropertyInfo;
    var accessor = propertyInfo.ConstructPropertyAccessor<TEntity, object>();
    valueAccessors.Add(accessor);
}
```

#### `ConstructPropertySetter`

This is an extension method that should construct an expression for setting the value of a specific property.
Usually, it is a good practice to minimize the reflection calls in code. One way of achieving this is throughout expressions (that are constructed and compiled only once for the lifetime of a program).
This expression method can be easily used with the `IMembersBinder` we described in the previous chapter.
Conversions are also handled, e.g. a common use case is to set the values of all properties after unboxing `object` instances - this can be achieved without any additional configurations as long as the required conversion can be executed.

Example:
```C#
IMembersBinder binder = new MembersBinder<TEntity>(x => x is PropertyInfo { CanWrite: true}, BindingFlags.Public | BindingFlags.Instance);
List<Expression<Action<TEntity, object>>> valueSetters = new List<Expression<Action<TEntity, object>>>();

foreach (var (_, memberInfo) in binder.MemberInfos)
{
    var propertyInfo = memberInfo as PropertyInfo;
    var setter = propertyInfo.ConstructPropertySetter<TEntity, object>();
    valueSetters.Add(setter);
}
```

### Generic extensions

#### `ExtractGenericParametersSetup`

This method will produce a dictionary where against the name of a generic parameter is stored the actual type associated with that parameter according to some external configuration.
This method should be used whenever each generic parameter is uniquely identified with a certain attribute.
All entries within the external configuration map should have as a key the attribute type and as a value the type that should be substituted for a generic parameter decorated with the corresponding attribute.

If there are none or multiple attributes for a generic parameter, this method will throw an exception.
If the external configuration is missing some attribute type, an exception will be thrown as well.

Example:
```C#
public class MyType<[KeyType] TKey, [ValueType] TValue> {}

IDictionary<Type, Type> typesMap = new Dictionary<Type, Type> { { typeof(KeyTypeAttribute), typeof(int) }, { typeof(ValueTypeAttribute), typeof(string) } };

/// should return { "TKey": typeof(int), "TValue": typeof(string) }
IDictionary<string, Type> genericParametersSetup = ExtractGenericParametersSetup(typeof(MyType<,>), typesMap);
```

#### `MakeGenericType`

Use this method to make the extended type generic using a parameters setup.

Example:
```C#
public class MyType<[KeyType] TKey, [ValueType] TValue> {}

IDictionary<Type, Type> typesMap = new Dictionary<Type, Type> { { typeof(KeyTypeAttribute), typeof(int) }, { typeof(ValueTypeAttribute), typeof(string) } };
IDictionary<string, Type> genericParametersSetup = ExtractGenericParametersSetup(typeof(MyType<,>), typesMap);
Type genericType = typeof(MyType<,>).MakeGenericType(genericParametersSetup);
```