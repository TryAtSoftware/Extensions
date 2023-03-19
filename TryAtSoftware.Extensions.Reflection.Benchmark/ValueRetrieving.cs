namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                               Method |       N |         Mean |      Error |     StdDev |
|------------------------------------- |-------- |-------------:|-----------:|-----------:|
| RetrievePropertyValueUsingReflection |   10000 |    148.72 us |   1.545 us |   1.446 us |
|   RetrievePropertyValueUsingDelegate |   10000 |     20.97 us |   0.155 us |   0.137 us |
| RetrievePropertyValueUsingExpression |   10000 |     11.87 us |   0.098 us |   0.091 us |
| RetrievePropertyValueUsingReflection |   50000 |    740.42 us |  10.090 us |   9.909 us |
|   RetrievePropertyValueUsingDelegate |   50000 |    105.12 us |   0.849 us |   0.663 us |
| RetrievePropertyValueUsingExpression |   50000 |     59.09 us |   0.360 us |   0.319 us |
| RetrievePropertyValueUsingReflection |  100000 |  1,460.23 us |  10.921 us |   9.681 us |
|   RetrievePropertyValueUsingDelegate |  100000 |    209.75 us |   3.737 us |   3.121 us |
| RetrievePropertyValueUsingExpression |  100000 |    116.96 us |   0.973 us |   0.863 us |
| RetrievePropertyValueUsingReflection |  500000 |  7,281.88 us |  45.642 us |  38.113 us |
|   RetrievePropertyValueUsingDelegate |  500000 |  1,039.40 us |   6.644 us |   5.889 us |
| RetrievePropertyValueUsingExpression |  500000 |    591.68 us |   7.852 us |   6.557 us |
| RetrievePropertyValueUsingReflection | 1000000 | 14,591.68 us | 182.054 us | 142.136 us |
|   RetrievePropertyValueUsingDelegate | 1000000 |  2,071.31 us |  12.391 us |  11.591 us |
| RetrievePropertyValueUsingExpression | 1000000 |  1,186.30 us |  20.979 us |  22.447 us |
 */
public class ValueRetrieving
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Func<BenchmarkModel, int> _compiledExpression = null!;
    private Func<BenchmarkModel, int> _compiledDelegate = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(BenchmarkModel).GetProperty(nameof(BenchmarkModel.Value))!;

        var valueRetrievingExpression = this._propertyInfo.ConstructPropertyAccessor<BenchmarkModel, int>();
        this._compiledExpression = valueRetrievingExpression.Compile();

        this._compiledDelegate = this._propertyInfo.GetMethod!.CreateDelegate<Func<BenchmarkModel, int>>();
    }

    [Benchmark]
    public void RetrievePropertyValueUsingReflection()
    {
        for (var i = 0; i < this.N; i++) _ = this._propertyInfo.GetValue(this._benchmarkModel);
    }

    [Benchmark]
    public void RetrievePropertyValueUsingDelegate()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledDelegate(this._benchmarkModel);
    }

    [Benchmark]
    public void RetrievePropertyValueUsingExpression()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpression(this._benchmarkModel);
    }
}