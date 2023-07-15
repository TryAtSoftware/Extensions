namespace TryAtSoftware.Extensions.DependencyInjection.Options;

using System;
using System.Collections.Generic;

public class RegisterServiceOptions
{
    public IDictionary<Type, Type>? GenericTypesMap { get; set; }
}