namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                               Method |          Mean |       Error |      StdDev |
|------------------------------------- |--------------:|------------:|------------:|
| RetrievePropertyValueUsingReflection |     13.777 ns |   0.1390 ns |   0.1301 ns |
|   RetrievePropertyValueUsingDelegate |      1.861 ns |   0.0320 ns |   0.0267 ns |
| RetrievePropertyValueUsingExpression |      1.331 ns |   0.0551 ns |   0.0634 ns |
|       CompileValueRetrievingDelegate |    305.024 ns |   2.0415 ns |   1.8098 ns |
|     CompileValueRetrievingExpression | 44,375.068 ns | 733.1727 ns | 814.9195 ns |
 */
public class ValueRetrieving
{
    private readonly Model _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Func<Model, int> _compiledExpression = null!;
    private Func<Model, int> _compiledDelegate = null!;

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(Model).GetProperty(nameof(Model.Value))!;

        var valueRetrievingExpression = this._propertyInfo.ConstructPropertyAccessor<Model, int>();
        this._compiledExpression = valueRetrievingExpression.Compile();

        this._compiledDelegate = this._propertyInfo.GetMethod!.CreateDelegate<Func<Model, int>>();
    }

    [Benchmark]
    public void RetrievePropertyValueUsingReflection() => _ = this._propertyInfo.GetValue(this._benchmarkModel);

    [Benchmark]
    public void RetrievePropertyValueUsingDelegate() => _ = this._compiledDelegate(this._benchmarkModel);

    [Benchmark]
    public void RetrievePropertyValueUsingExpression() => _ = this._compiledExpression(this._benchmarkModel);

    [Benchmark]
    public void CompileValueRetrievingDelegate() => _ = this._propertyInfo.GetMethod!.CreateDelegate<Func<Model, int>>();

    [Benchmark]
    public void CompileValueRetrievingExpression() => _ = this._propertyInfo.ConstructPropertyAccessor<Model, int>().Compile();
}