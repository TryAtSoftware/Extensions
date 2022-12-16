namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System;
using System.Collections.Generic;
using System.Reflection;

public interface IMembersBinder
{
    Type Type { get; }
    IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }
}