namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                               Method |       N |         Mean |     Error |    StdDev |
|------------------------------------- |-------- |-------------:|----------:|----------:|
| RetrievePropertyValueUsingReflection |   10000 |    144.56 us |  2.799 us |  3.223 us |
| RetrievePropertyValueUsingExpression |   10000 |     11.44 us |  0.061 us |  0.057 us |
| RetrievePropertyValueUsingReflection |   50000 |    705.24 us |  6.863 us |  6.420 us |
| RetrievePropertyValueUsingExpression |   50000 |     56.75 us |  0.224 us |  0.175 us |
| RetrievePropertyValueUsingReflection |  100000 |  1,410.36 us |  9.666 us |  8.569 us |
| RetrievePropertyValueUsingExpression |  100000 |    113.54 us |  0.673 us |  0.562 us |
| RetrievePropertyValueUsingReflection |  500000 |  7,187.74 us | 96.072 us | 80.225 us |
| RetrievePropertyValueUsingExpression |  500000 |    567.66 us |  3.361 us |  3.144 us |
| RetrievePropertyValueUsingReflection | 1000000 | 13,973.26 us | 77.321 us | 72.326 us |
| RetrievePropertyValueUsingExpression | 1000000 |  1,149.74 us | 21.910 us | 23.444 us |
 */
public class ValueRetrieving
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Func<BenchmarkModel, int> _compiledExpression = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(BenchmarkModel).GetProperty(nameof(BenchmarkModel.Value)) ?? throw new InvalidOperationException("Property was not successfully retrieved.");

        var valueRetrievingExpression = this._propertyInfo.ConstructPropertyAccessor<BenchmarkModel, int>();
        this._compiledExpression = valueRetrievingExpression.Compile();
    }

    [Benchmark]
    public void RetrievePropertyValueUsingReflection()
    {
        for (var i = 0; i < this.N; i++) _ = this._propertyInfo.GetValue(this._benchmarkModel);
    }

    [Benchmark]
    public void RetrievePropertyValueUsingExpression()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpression(this._benchmarkModel);
    }
}