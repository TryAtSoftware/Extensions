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
    /// If the filter determines that a given assembly should not be loaded, none of the assemblies it references (including the referenced assemblies of the referenced assemblies, and so on) will be loaded as well.
    /// This property is nullable, meaning it can be assigned a value or left as <c>null</c> if no filtering is required.
    /// </summary>
    public Func<AssemblyName, bool>? RestrictSearchFilter { get; set; }
}