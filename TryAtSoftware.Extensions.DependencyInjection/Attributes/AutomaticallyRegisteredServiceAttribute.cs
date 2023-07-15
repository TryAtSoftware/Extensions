namespace TryAtSoftware.Extensions.DependencyInjection.Attributes;

using System;

/// <summary>
/// An attribute that should be used to decorate all services that are expected to be automatically registered into a dependency injection contained.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = Constants.RegistrationIsInheritable)]
public class AutomaticallyRegisteredServiceAttribute : Attribute
{
}