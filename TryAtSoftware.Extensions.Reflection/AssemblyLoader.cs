namespace TryAtSoftware.Extensions.Reflection;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// A default implementation of the <see cref="IAssemblyLoader"/> interface.
/// </summary>
/// <remarks>It is used as a wrapper around the static <see cref="Assembly.Load(AssemblyName)"/> method.</remarks>
[ExcludeFromCodeCoverage]
public class AssemblyLoader : IAssemblyLoader
{
    /// <inheritdoc />
    public Assembly? Load(AssemblyName assemblyName) => Assembly.Load(assemblyName);
}