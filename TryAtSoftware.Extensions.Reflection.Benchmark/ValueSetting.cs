namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                          Method |       N |         Mean |      Error |       StdDev |       Median |
|-------------------------------- |-------- |-------------:|-----------:|-------------:|-------------:|
| SetPropertyValueUsingReflection |   10000 |    333.61 us |   3.042 us |     2.697 us |    333.61 us |
|   SetPropertyValueUsingDelegate |   10000 |     21.19 us |   0.149 us |     0.117 us |     21.15 us |
| SetPropertyValueUsingExpression |   10000 |     14.24 us |   0.285 us |     0.279 us |     14.12 us |
| SetPropertyValueUsingReflection |   50000 |  1,628.11 us |  10.197 us |     9.539 us |  1,628.33 us |
|   SetPropertyValueUsingDelegate |   50000 |    105.89 us |   0.728 us |     0.569 us |    105.86 us |
| SetPropertyValueUsingExpression |   50000 |     71.48 us |   1.403 us |     1.616 us |     70.75 us |
| SetPropertyValueUsingReflection |  100000 |  3,342.42 us |  65.056 us |    79.894 us |  3,311.60 us |
|   SetPropertyValueUsingDelegate |  100000 |    214.15 us |   4.048 us |     4.662 us |    212.93 us |
| SetPropertyValueUsingExpression |  100000 |    140.58 us |   0.958 us |     0.800 us |    140.35 us |
| SetPropertyValueUsingReflection |  500000 | 17,328.19 us | 341.627 us |   658.199 us | 17,009.09 us |
|   SetPropertyValueUsingDelegate |  500000 |  1,048.16 us |   6.594 us |     5.507 us |  1,046.56 us |
| SetPropertyValueUsingExpression |  500000 |    695.58 us |  10.516 us |     8.782 us |    691.18 us |
| SetPropertyValueUsingReflection | 1000000 | 34,706.94 us | 691.861 us | 1,247.568 us | 35,029.51 us |
|   SetPropertyValueUsingDelegate | 1000000 |  2,090.67 us |  13.542 us |    12.667 us |  2,091.11 us |
| SetPropertyValueUsingExpression | 1000000 |  1,387.18 us |  21.329 us |    17.811 us |  1,382.35 us |
 */
public class ValueSetting
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Action<BenchmarkModel, int> _compiledExpression = null!;
    private Action<BenchmarkModel, int> _compiledDelegate = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(BenchmarkModel).GetProperty(nameof(BenchmarkModel.Value))!;

        var valueSettingExpression = this._propertyInfo.ConstructPropertySetter<BenchmarkModel, int>();
        this._compiledExpression = valueSettingExpression.Compile();
        this._compiledDelegate = this._propertyInfo.SetMethod!.CreateDelegate<Action<BenchmarkModel, int>>();
    }

    [Benchmark]
    public void SetPropertyValueUsingReflection()
    {
        for (var i = 0; i < this.N; i++) this._propertyInfo.SetValue(this._benchmarkModel, i);
    }

    [Benchmark]
    public void SetPropertyValueUsingDelegate()
    {
        for (var i = 0; i < this.N; i++) this._compiledDelegate(this._benchmarkModel, i);
    }

    [Benchmark]
    public void SetPropertyValueUsingExpression()
    {
        for (var i = 0; i < this.N; i++) this._compiledExpression(this._benchmarkModel, i);
    }
}