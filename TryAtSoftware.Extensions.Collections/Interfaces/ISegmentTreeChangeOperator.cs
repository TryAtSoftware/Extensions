namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTreeChangeOperator<TValue>
{
    TValue ApplyChange(TValue currentValue);
}