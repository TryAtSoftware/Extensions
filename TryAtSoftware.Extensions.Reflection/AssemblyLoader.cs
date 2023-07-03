namespace TryAtSoftware.Extensions.Reflection;

using System.Reflection;
using TryAtSoftware.Extensions.Reflection.Interfaces;

public class AssemblyLoader : IAssemblyLoader
{
    public Assembly? Load(AssemblyName assemblyName) => Assembly.Load(assemblyName);
}