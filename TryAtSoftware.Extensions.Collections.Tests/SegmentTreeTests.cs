namespace TryAtSoftware.Extensions.Collections.Tests;

using TryAtSoftware.Extensions.Collections.Interfaces;
using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

public static class SegmentTreeTests
{
    [Fact]
    public static void SegmentTreeShouldBeUpdatedSuccessfully()
    {
        var n = RandomizationHelper.RandomInteger(0, 100);
        
        var initializationEngine = new StandardSegmentTreeInitializationEngine<int>(0);
        var segmentTree = new RecursiveSegmentTree<int>(n, initializationEngine);

        var numbers = new int[n];
        for (var i = 0; i < n; i++)
        {
            numbers[i] = RandomizationHelper.RandomInteger(0, 100);
            segmentTree.Update(i, new StandardSegmentTreeChangeOperator<int>(numbers[i]));
        }

        ValidateQueries(numbers, segmentTree);
    }
    
    [Fact]
    public static void SegmentTreeShouldBeLazyUpdatedSuccessfully()
    {
        var n = RandomizationHelper.RandomInteger(0, 100);
        
        var initializationEngine = new StandardSegmentTreeInitializationEngine<int>(0);
        var segmentTree = new RecursiveSegmentTree<int>(n, initializationEngine);

        var numbers = new int[n];
        for (var i = 0; i < n; i++)
        {
            numbers[i] = RandomizationHelper.RandomInteger(0, 100);
            segmentTree.LazyUpdate(i, new StandardSegmentTreeChangeOperator<int>(numbers[i]));
        }

        ValidateQueries(numbers, segmentTree);
    }

    private static void ValidateQueries(int[] numbers, RecursiveSegmentTree<int> segmentTree)
    {
        var queryEngine = new SumSegmentTreeQueryOperator();
        for (var i = 0; i < numbers.Length; i++) Assert.Equal(numbers[i], segmentTree.Query(i, queryEngine));

        var prefixSum = new int[numbers.Length + 1];
        for (var i = 0; i < numbers.Length; i++) prefixSum[i + 1] = prefixSum[i] + numbers[i];

        for (var i = 0; i < numbers.Length; i++)
            for (var j = i; j < numbers.Length; j++) Assert.Equal(prefixSum[j + 1] - prefixSum[i], segmentTree.Query(i, j, queryEngine));
    }
}

public class StandardSegmentTreeInitializationEngine<TValue>(TValue defaultValue) : ISegmentTreeInitializationEngine<TValue>
{
    public TValue CreateInitialValue(int index) => defaultValue;
}

public class StandardSegmentTreeChangeOperator<TValue>(TValue newValue) : ISegmentTreeChangeOperator<TValue>
{
    public TValue ApplyChange(TValue currentValue) => newValue;
}

public class SumSegmentTreeQueryOperator : ISegmentTreeQueryOperator<int, int>
{
    public int Merge(int left, int right) => left + right;

    public int ProduceResult(int value) => value;
}