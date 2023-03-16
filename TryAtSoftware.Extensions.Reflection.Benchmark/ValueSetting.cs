namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                          Method |       N |         Mean |      Error |       StdDev |       Median |
|-------------------------------- |-------- |-------------:|-----------:|-------------:|-------------:|
| SetPropertyValueUsingReflection |   10000 |    327.84 us |   1.940 us |     1.720 us |    327.93 us |
| SetPropertyValueUsingExpression |   10000 |     11.37 us |   0.035 us |     0.031 us |     11.37 us |
| SetPropertyValueUsingReflection |   50000 |  1,561.69 us |  17.282 us |    14.431 us |  1,559.26 us |
| SetPropertyValueUsingExpression |   50000 |     56.73 us |   0.629 us |     0.588 us |     56.39 us |
| SetPropertyValueUsingReflection |  100000 |  3,072.71 us |   9.992 us |     8.344 us |  3,073.82 us |
| SetPropertyValueUsingExpression |  100000 |    113.21 us |   0.927 us |     0.774 us |    112.92 us |
| SetPropertyValueUsingReflection |  500000 | 15,677.52 us | 105.741 us |    98.910 us | 15,662.07 us |
| SetPropertyValueUsingExpression |  500000 |    566.84 us |   3.425 us |     3.036 us |    565.96 us |
| SetPropertyValueUsingReflection | 1000000 | 31,877.39 us | 635.977 us | 1,146.798 us | 31,247.00 us |
| SetPropertyValueUsingExpression | 1000000 |  1,131.43 us |   3.365 us |     2.983 us |  1,130.91 us |
 */
public class ValueSetting
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Action<BenchmarkModel, int> _compiledExpression = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(BenchmarkModel).GetProperty(nameof(BenchmarkModel.Value)) ?? throw new InvalidOperationException("Property was not successfully retrieved.");

        var valueSettingExpression = this._propertyInfo.ConstructPropertySetter<BenchmarkModel, int>();
        this._compiledExpression = valueSettingExpression.Compile();
    }

    [Benchmark]
    public void SetPropertyValueUsingReflection()
    {
        for (var i = 0; i < this.N; i++) this._propertyInfo.SetValue(this._benchmarkModel, i);
    }

    [Benchmark]
    public void SetPropertyValueUsingExpression()
    {
        for (var i = 0; i < this.N; i++) this._compiledExpression(this._benchmarkModel, i);
    }
}