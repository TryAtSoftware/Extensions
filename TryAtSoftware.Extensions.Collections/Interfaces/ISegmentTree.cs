namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTree<TValue>
{
    TOutput Query<TOutput>(int index, ISegmentTreeQueryOperator<TValue, TOutput> queryOperator);
    TOutput Query<TOutput>(int start, int end, ISegmentTreeQueryOperator<TValue, TOutput> queryOperator);

    void Update(int index, ISegmentTreeChangeOperator<TValue> changeOperator);
    void Update(int start, int end, ISegmentTreeChangeOperator<TValue> changeOperator);

    void LazyUpdate(int index, ISegmentTreeChangeOperator<TValue> changeOperator);
    void LazyUpdate(int start, int end, ISegmentTreeChangeOperator<TValue> changeOperator);
}