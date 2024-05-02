namespace TryAtSoftware.Extensions.Collections.Tests;

using TryAtSoftware.Randomizer.Core.Helpers;
using Xunit;

#if NET7_0_OR_GREATER

public static class SegmentTreeTests
{
    [Fact]
    public static void SegmentTreeShouldBeUpdatedSuccessfully()
    {
        var n = RandomizationHelper.RandomInteger(0, 100);
        
        var engine = new StandardSegmentTreeSumEngine<int>();
        var segmentTree = new RecursiveSegmentTree<int, int, int>(engine, n);

        var numbers = new int[n];
        for (var i = 0; i < n; i++)
        {
            numbers[i] = RandomizationHelper.RandomInteger(0, 100);
            segmentTree.Update(i, numbers[i]);
        }

        for (var i = 0; i < n; i++) Assert.Equal(numbers[i], segmentTree.Query(i));

        var prefixSum = new int[n + 1];
        for (var i = 0; i < n; i++) prefixSum[i + 1] = prefixSum[i] + numbers[i];

        for (var i = 0; i < n; i++)
            for (var j = i; j < n; j++) Assert.Equal(prefixSum[j + 1] - prefixSum[i], segmentTree.Query(i, j));
    }
}

#endif