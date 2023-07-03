namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.Reflection.Options;

public static class AssemblyExtensions
{
    public static void LoadReferencedAssemblies(this Assembly assembly, LoadReferencedAssembliesOptions? options = null)
    {
        if (assembly is null) throw new ArgumentNullException(nameof(assembly));
        
        LoadReferencedAssemblies(assembly, new HashSet<string>(), options ?? new LoadReferencedAssembliesOptions());
    }

    private static void LoadReferencedAssemblies(this Assembly assembly, ISet<string> iteratedAssemblies, LoadReferencedAssembliesOptions options)
    {
        foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies().OrEmptyIfNull().IgnoreNullValues())
        {
            var currentAssemblyName = referencedAssemblyName.ToString();
            if (!iteratedAssemblies.Add(currentAssemblyName) || (options.RestrictSearchFilter is not null && !options.RestrictSearchFilter(referencedAssemblyName))) continue;

            var referencedAssembly = options.Loader.Load(referencedAssemblyName);
            if (referencedAssembly is not null) LoadReferencedAssemblies(referencedAssembly, iteratedAssemblies, options);
        }
    }
}