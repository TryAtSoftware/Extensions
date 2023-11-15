[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=TryAtSoftware_Extensions&metric=bugs)](https://sonarcloud.io/summary/new_code?id=TryAtSoftware_Extensions)

[![Core validation](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml/badge.svg)](https://github.com/TryAtSoftware/Extensions/actions/workflows/Core%20validation.yml)

# About the project

`TryAtSoftware.Extensions.Reflection` is a library containing extension methods and utility components that should simplify (and optimize) some common operations with reflection.

# About us

`Try At Software` is a software development company based in Bulgaria. We are mainly using `dotnet` technologies (`C#`, `ASP.NET Core`, `Entity Framework Core`, etc.) and our main idea is to provide a set of tools that can simplify the majority of work a developer does on a daily basis.

# Getting started

## Installing the package

In order to use this library, you need to install the corresponding NuGet package beforehand.
The simplest way to do this is to either use the `NuGet package manager`, or the `dotnet CLI`.

Using the `NuGet package manager` console within Visual Studio, you can install the package using the following command:

> Install-Package TryAtSoftware.Extensions.Reflection

Or using the `dotnet CLI` from a terminal window:

> dotnet add package TryAtSoftware.Extensions.Reflection

## `TypeNames`

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

## `IMembersBinder`

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
// IMembersBinder binder = new MembersBinder(typeof(TEntity), IsValidMember, BindingFlags.Public | BindingFlags.Instance);

static bool IsValidMember(MemberInfo memberInfo)
    => memberInfo switch
    {
        PropertyInfo pi => pi.CanWrite,
        FieldInfo _ => true,
        _ => false
    };

// Now every discovered member for the `TEntity` type will be mapped against its name throughout the `binder.MemberInfos` dictionary.
```

### Members binder and extended interfaces

It is known that whenever the `.GetMembers()` method is invoked for an interface type, none of the members defined in extended interfaces will be returned.
The default implementations of the `IMembersBinder` method overcome this and will include members from all of the extended interfaces by recursively retrieving all of them.
That being said, it is obvious that the `ReflectedType` in this case may not equal the `Type` of the `IMembersBinder` instance.

## `IHierarchyScanner`

This is an interface defining the structure of a component responsible for scanning type hierarchies.
A common application of this interface and its corresponding implementing types is to scan a type hierarchy for a given attribute.

It is a common practice to decorate methods or properties with attributes in order to model some kind of behavior related to them.
But it often becomes inconvenient to use the same attribute(s) over and over again for all methods or properties within a given class or assembly. 
A better solution would be to define the attribute(s) at a higher level.
And this is where the `IHierarchyScanner` may be used - it can **scan** for attributes throughout the type hierarchy, allowing for effortless implementation, management and control.

The default implementation would **scan** the requested member and its type but this can be easily extend to include the assembly as well (if the attribute is inheritable, base types and overriden methods will be included as well).
The result set should always be ordered hierarchically!

Example:

```C#
IHierarchyScanner hierarchyScanner = new HierarchyScanner();
IReadOnlyCollection<MyAttribute> attributes = hierarchyScanner.ScanForAttribute<MyAttribute>(memberInfo);
```

## Assembly extensions

### `LoadReferencedAssemblies`

This is an extension method that should be used to to recursively load referenced assemblies, starting from the extended `Assembly` instance.
A common use case is to discovered types via reflection from assemblies that are not yet loaded within the application domain.

Additionally, a `LoadReferencedAssembliesOptions` instance can be provided to refine process of loading referenced assemblies.
It exposes two configurable parameters:
- `Loader` - An `IAssemblyLoader` instance controlling how exactly the referenced assemblies will be loaded.
- `RestrictSearchFilter` - A filter defining which assemblies should be loaded.
  If the filter determines that a given assembly should not be loaded, this serves as a terminating case for the recursion, and thus none of the assemblies it references (directly or transitively) will be loaded.


Example:

```C#
LoadReferencedAssembliesOptions options = new LoadReferencedAssembliesOptions
{
    RestrictSearchFilter = (assemblyName) => assemblyName.FullName.StartsWith("Common.Assembly.Name.Prefix")
};

Assembly.GetExecutingAssembly().LoadReferencedAssemblies(options);
```

## Expression extensions

### `ConstructPropertyAccessor`

This is an extension method that should construct an expression for accessing the value of a specific property.
Usually, it is a good practice to minimize the reflection calls in code. One way of achieving this is throughout expressions (that are constructed and compiled only once for the lifetime of a program).
This expression method can be easily used with the `IMembersBinder` we described in the previous chapter.
Conversions are also handled, e.g. a common use case is to retrieve the values of all properties by boxing them as `object` instances - this can be achieved without any additional configurations as long as the required conversion can be executed.

Example:

```C#
IMembersBinder binder = new MembersBinder<TEntity>(x => x is PropertyInfo { CanRead: true }, BindingFlags.Public | BindingFlags.Instance);
List<Expression<Func<TEntity, object>>> valueAccessors = new List<Expression<Func<TEntity, object>>>();

foreach (var (_, memberInfo) in binder.MemberInfos)
{
    var propertyInfo = memberInfo as PropertyInfo;
    var accessor = propertyInfo.ConstructPropertyAccessor<TEntity, object>();
    valueAccessors.Add(accessor);
}
```

### `ConstructPropertySetter`

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

### `ConstructObjectInitializer`

This is an extension method that should construct an expression for instantiating an object using a specific constructor.
Usually, it is a good practice to minimize the reflection calls in code. One way of achieving this is throughout expressions (that are constructed and compiled only once for the lifetime of a program).
This expression method can be easily used with the `IMembersBinder` we described in the previous chapter.

This methods accept one optional parameter called `includeParametersCountValidation`. It indicates whether or not the subsequently constructed expression should include a check to validate the correct count of provided values.

An expression constructed by this extension method can be compiled to a function that accepts an array of values that correspond to the parameters of the extended `ConstructorInfo` instance.
If any of the parameters is optional and its default value should be used, the corresponding element (from the provided array) must be `null`.

Example:

```C#
IMembersBinder binder = new MembersBinder<TEntity>(x => x is ConstructorInfo, x => PrepareConstructorKey((ConstructorInfo)x), BindingFlags.Public | BindingFlags.Instance);
List<Expression<Func<object?[], TEntity>>> objectInitializers = new List<Expression<Func<object?[], TEntity>>>();

foreach (var (_, memberInfo) in binder.MemberInfos)
{
    var constructorInfo = memberInfo as ConstructorInfo;
    var objectInitializer = constructorInfo.ConstructObjectInitializer<TEntity>();
    objectInitializers.Add(objectInitializer);
}

static string PrepareConstructorKey(ConstructorInfo constructorInfo)
{
    var parameterTypeNames = constructorInfo.GetParameters().Select(p => TypeNames.Get(p.ParameterType));
    return $"Constructor[{string.Join(", ", parameterTypeNames)}]";
}
```

## Generic extensions

### `ExtractGenericParametersSetup`

Use this method to extract the setup of generic parameters for a given type.

This method will produce a dictionary where against the name of each generic parameter will be mapped the `actual type` that should substitute it.
In order to use it, each generic parameter must be uniquely identified with an attribute.

It is required to provide a one-to-one mapping between the `attribute type` and the `actual type` that should substitute the decorated generic parameter(s).

> If **none** or **multiple** attributes, for which there exists an entry within the provided mapping, decorate a generic parameter, an exception will be thrown.

Example:

```C#
public class MyType<[KeyType] TKey, [ValueType] TValue> {}

IDictionary<Type, Type> typesMap = new Dictionary<Type, Type> { { typeof(KeyTypeAttribute), typeof(int) }, { typeof(ValueTypeAttribute), typeof(string) } };

// should return { "TKey": typeof(int), "TValue": typeof(string) }
IDictionary<string, Type> genericParametersSetup = ExtractGenericParametersSetup(typeof(MyType<,>), typesMap);
```

### `MakeGenericType`

Use this method to make the extended type generic using a parameters setup.

Example:

```C#
public class MyType<[KeyType] TKey, [ValueType] TValue> {}

IDictionary<Type, Type> typesMap = new Dictionary<Type, Type> { { typeof(KeyTypeAttribute), typeof(int) }, { typeof(ValueTypeAttribute), typeof(string) } };
IDictionary<string, Type> genericParametersSetup = ExtractGenericParametersSetup(typeof(MyType<,>), typesMap);
Type genericType = typeof(MyType<,>).MakeGenericType(genericParametersSetup);
```
