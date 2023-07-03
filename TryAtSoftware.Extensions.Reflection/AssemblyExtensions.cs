namespace TryAtSoftware.Extensions.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;
using TryAtSoftware.Extensions.Collections;

public static class AssemblyExtensions
{
    public static void LoadReferencedAssemblies(this Assembly assembly, Func<AssemblyName, bool>? restrictSearchFilter = null)
    {
        if (assembly is null) throw new ArgumentNullException(nameof(assembly));
        LoadReferencedAssemblies(assembly, new HashSet<string>(), restrictSearchFilter);
    }

    private static void LoadReferencedAssemblies(this Assembly assembly, HashSet<string> iteratedAssemblies, Func<AssemblyName, bool>? restrictSearchFilter)
    {
        if (assembly is null) throw new ArgumentNullException(nameof(assembly));
        if (iteratedAssemblies is null) throw new ArgumentNullException(nameof(iteratedAssemblies));

        foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies().OrEmptyIfNull().IgnoreNullValues())
        {
            var currentAssemblyName = referencedAssemblyName.ToString();
            if (!iteratedAssemblies.Add(currentAssemblyName) || (restrictSearchFilter is not null && !restrictSearchFilter(referencedAssemblyName))) continue;

            var referencedAssembly = Assembly.Load(referencedAssemblyName);
            LoadReferencedAssemblies(referencedAssembly, iteratedAssemblies, restrictSearchFilter);
        }
    }
}