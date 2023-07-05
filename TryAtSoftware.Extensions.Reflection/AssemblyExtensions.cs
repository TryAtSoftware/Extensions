namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;
using TryAtSoftware.Extensions.Collections;
using TryAtSoftware.Extensions.Reflection.Options;

/// <summary>
/// A static class containing standard extension methods that are useful when executing common operations with assemblies.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Use this method to recursively load referenced assemblies, starting from the extended <paramref name="assembly"/>. 
    /// </summary>
    /// <param name="assembly">The extended <see cref="Assembly"/> instance.</param>
    /// <param name="options">The options used to refine the process of loading referenced assemblies. If not provided, a default <see cref="LoadReferencedAssembliesOptions"/> instance will be used.</param>
    /// <exception cref="ArgumentNullException">Thrown if the extended <paramref name="assembly"/> is <c>null</c>.</exception>
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