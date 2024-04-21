namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTreeEngine<TValue, TOutput, TChange>
{
    TValue CreateDefaultValue();
    TChange Combine(TChange pendingChange, TChange newChange);
    TValue ApplyChange(TValue currentValue, TChange change);
    
    TOutput Merge(TOutput left, TOutput right);
    TOutput ProduceResult(TValue value);
}