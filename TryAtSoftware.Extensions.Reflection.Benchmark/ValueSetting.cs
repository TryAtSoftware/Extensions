namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                          Method |           Mean |       Error |      StdDev |
|-------------------------------- |---------------:|------------:|------------:|
| SetPropertyValueUsingReflection |     31.6062 ns |   0.3097 ns |   0.2586 ns |
|   SetPropertyValueUsingDelegate |      1.6304 ns |   0.0228 ns |   0.0191 ns |
| SetPropertyValueUsingExpression |      0.5817 ns |   0.0408 ns |   0.0382 ns |
|     CompileValueSettingDelegate |    346.5711 ns |   6.5976 ns |   5.8486 ns |
|   CompileValueSettingExpression | 47,610.6336 ns | 225.6740 ns | 200.0541 ns |
 */
public class ValueSetting
{
    private readonly Model _benchmarkModel = new () { Value = 1024 };
    private PropertyInfo _propertyInfo = null!;
    private Action<Model, int> _compiledExpression = null!;
    private Action<Model, int> _compiledDelegate = null!;

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(Model).GetProperty(nameof(Model.Value))!;

        var valueSettingExpression = this._propertyInfo.ConstructPropertySetter<Model, int>();
        this._compiledExpression = valueSettingExpression.Compile();
        this._compiledDelegate = this._propertyInfo.SetMethod!.CreateDelegate<Action<Model, int>>();
    }

    [Benchmark]
    public void SetPropertyValueUsingReflection() => this._propertyInfo.SetValue(this._benchmarkModel, 314);

    [Benchmark]
    public void SetPropertyValueUsingDelegate() => this._compiledDelegate(this._benchmarkModel, 314);

    [Benchmark]
    public void SetPropertyValueUsingExpression() => this._compiledExpression(this._benchmarkModel, 314);

    [Benchmark]
    public void CompileValueSettingDelegate() => _ = this._propertyInfo.SetMethod!.CreateDelegate<Action<Model, int>>();

    [Benchmark]
    public void CompileValueSettingExpression() => _ =  this._propertyInfo.ConstructPropertySetter<Model, int>().Compile();
}