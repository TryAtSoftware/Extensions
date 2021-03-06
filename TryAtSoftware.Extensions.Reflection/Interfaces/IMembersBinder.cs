namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

public interface IMembersBinder<[UsedImplicitly] TEntity>
{
    [NotNull]
    IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }
}