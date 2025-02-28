namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTreeQueryOperator<in TValue, TOutput>
{
    TOutput Merge(TOutput left, TOutput right);
    TOutput ProduceResult(TValue value);
}