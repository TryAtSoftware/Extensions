namespace TryAtSoftware.Extensions.Collections;

#if NET7_0_OR_GREATER

using System.Numerics;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class StandardSegmentTreeSumEngine<T> : ISegmentTreeEngine<T, T, T>
    where T : struct, INumber<T>
{
    public T CreateDefaultValue() => default;

    public T Combine(T pendingChange, T newChange) => newChange;

    public T ApplyChange(T currentValue, T change) => change;

    public T Merge(T left, T right) => left + right;

    public T ProduceResult(T value) => value;
}

#endif