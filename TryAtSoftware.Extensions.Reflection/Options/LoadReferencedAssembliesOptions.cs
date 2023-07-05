namespace TryAtSoftware.Extensions.Reflection.Options;

using System;
using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public class LoadReferencedAssembliesOptions
{
    private IAssemblyLoader _loader = new AssemblyLoader();

    public IAssemblyLoader Loader
    {
        get => this._loader;
        set => this._loader = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Func<AssemblyName, bool>? RestrictSearchFilter { get; set; }
}