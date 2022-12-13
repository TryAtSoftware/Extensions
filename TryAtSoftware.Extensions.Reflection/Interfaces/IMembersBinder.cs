namespace TryAtSoftware.Extensions.Reflection.Interfaces;

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

public interface IMembersBinder
{
    [NotNull] Type Type { get; }
    [NotNull] IReadOnlyDictionary<string, MemberInfo> MemberInfos { get; }
}

public interface IMembersBinder<[UsedImplicitly] TEntity> : IMembersBinder
{
}