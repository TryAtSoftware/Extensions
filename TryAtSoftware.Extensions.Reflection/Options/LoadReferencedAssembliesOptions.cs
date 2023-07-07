namespace TryAtSoftware.Extensions.Reflection.Options;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

/// <summary>
/// A class representing the options that can be used to refine the process of loading referenced assemblies.
/// </summary>
public class LoadReferencedAssembliesOptions
{
    private IAssemblyLoader _loader = new AssemblyLoader();

    /// <summary>
    /// Gets or sets an instance of the <see cref="IAssemblyLoader"/> interface used to load assemblies.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when attempting to set <c>null</c> value.</exception>
    public IAssemblyLoader Loader
    {
        get => this._loader;
        set => this._loader = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets an optional filter function used to control which assemblies should be loaded.
    /// If the filter determines that a given assembly should not be loaded, this serves as a terminating case for the recursion, and thus none of the assemblies it references (directly or transitively) will be loaded.
    /// This property is nullable, meaning it can be assigned a value or left as <c>null</c> if no filtering is required.
    /// </summary>
    public Func<AssemblyName, bool>? RestrictSearchFilter { get; set; }
}