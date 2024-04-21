namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTree<out TOutput, in TChange>
{
    TOutput Query(int index);
    TOutput Query(int start, int end);

    void Update(int index, TChange change);
    void Update(int start, int end, TChange change);

    void LazyUpdate(int index, TChange change);
    void LazyUpdate(int start, int end, TChange change);
}