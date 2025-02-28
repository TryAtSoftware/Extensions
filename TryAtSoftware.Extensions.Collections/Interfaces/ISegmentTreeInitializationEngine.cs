namespace TryAtSoftware.Extensions.Collections.Interfaces;

public interface ISegmentTreeInitializationEngine<out TValue>
{
    TValue CreateInitialValue(int index);
}