namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System.Reflection;

public interface IAssemblyLoader
{
    Assembly? Load(AssemblyName assemblyName);
}