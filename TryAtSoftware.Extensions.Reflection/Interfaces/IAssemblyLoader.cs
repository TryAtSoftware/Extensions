namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System.Reflection;

/// <summary>
/// An interface defining the structure of a component responsible for loading assemblies.
/// </summary>
public interface IAssemblyLoader
{
    /// <summary>
    /// Use this method to load an assembly given its name.
    /// </summary>
    /// <param name="assemblyName">The object that describes the assembly to be loaded.</param>
    /// <returns>The loaded assembly.</returns>
    Assembly? Load(AssemblyName assemblyName);
}