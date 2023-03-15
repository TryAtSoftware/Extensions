namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;

public class ValueSetting
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Action<BenchmarkModel, int> _compiledExpression = null!;

    [Params(100, 1_000, 5_000, 10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

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