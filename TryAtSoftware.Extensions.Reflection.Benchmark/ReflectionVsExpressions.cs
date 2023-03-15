namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;

public class ReflectionVsExpressions
{
    private readonly BenchmarkModel _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Func<BenchmarkModel, int> _compiledExpression = null!;

    [Params(100, 1_000, 5_000, 10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

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