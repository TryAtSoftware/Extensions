namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                                              Method |         Mean |       Error |       StdDev |       Median |
|---------------------------------------------------- |-------------:|------------:|-------------:|-------------:|
|                      CompileValueRetrievingDelegate |     310.2 ns |     6.05 ns |      8.87 ns |     305.2 ns |
|                    CompileValueRetrievingExpression |  45,953.1 ns |   868.38 ns |    725.13 ns |  45,687.3 ns |
|                         CompileValueSettingDelegate |     346.8 ns |     5.28 ns |      4.68 ns |     345.1 ns |
|                       CompileValueSettingExpression |  49,361.5 ns |   481.88 ns |    402.39 ns |  49,301.8 ns |
|  CompileModelInstantiatingExpression_ZeroParameters |  50,282.6 ns |   770.06 ns |    601.21 ns |  50,196.4 ns |
|    CompileModelInstantiatingExpression_OneParameter | 120,172.8 ns |   988.34 ns |    876.14 ns | 120,027.3 ns |
|   CompileModelInstantiatingExpression_TwoParameters | 245,003.4 ns | 3,372.59 ns |  3,154.72 ns | 244,583.8 ns |
| CompileModelInstantiatingExpression_ThreeParameters | 313,465.6 ns | 5,602.12 ns | 12,983.78 ns | 312,135.3 ns |
|  CompileModelInstantiatingExpression_FourParameters | 376,898.9 ns | 5,819.94 ns |  4,859.91 ns | 376,028.5 ns |
*/
public class ExpressionsCompiling
{
    private PropertyInfo _propertyInfo = null!;
    private ConstructorInfo _emptyConstructorInfo = null!;
    private ConstructorInfo _oneParameterConstructorInfo = null!;
    private ConstructorInfo _twoParametersConstructorInfo = null!;
    private ConstructorInfo _threeParametersConstructorInfo = null!;
    private ConstructorInfo _fourParametersConstructorInfo = null!;

    [GlobalSetup]
    public void Setup()
    {
        this._propertyInfo = typeof(BenchmarkModel).GetProperty(nameof(BenchmarkModel.Value))!;
        
        this._emptyConstructorInfo = typeof(BenchmarkModelWithConstructors).GetConstructor(Array.Empty<Type>())!;
        this._oneParameterConstructorInfo = typeof(BenchmarkModelWithConstructors).GetConstructor(new[] { typeof(string) })!;
        this._twoParametersConstructorInfo = typeof(BenchmarkModelWithConstructors).GetConstructor(new[] { typeof(string), typeof(int) })!;
        this._threeParametersConstructorInfo = typeof(BenchmarkModelWithConstructors).GetConstructor(new[] { typeof(string), typeof(int), typeof(char) })!;
        this._fourParametersConstructorInfo = typeof(BenchmarkModelWithConstructors).GetConstructor(new[] { typeof(string), typeof(int), typeof(char), typeof(bool) })!;
    }

    [Benchmark]
    public void CompileValueRetrievingDelegate()
    {
        _ = this._propertyInfo.GetMethod!.CreateDelegate<Func<BenchmarkModel, int>>();
    }
    
    [Benchmark]
    public void CompileValueRetrievingExpression()
    {
        _ = this._propertyInfo.ConstructPropertyAccessor<BenchmarkModel, int>().Compile();
    }
    
    [Benchmark]
    public void CompileValueSettingDelegate()
    {
        _ = this._propertyInfo.SetMethod!.CreateDelegate<Action<BenchmarkModel, int>>();
    }
    
    [Benchmark]
    public void CompileValueSettingExpression()
    {
        _ =  this._propertyInfo.ConstructPropertySetter<BenchmarkModel, int>().Compile();
    }

    [Benchmark]
    public void CompileModelInstantiatingExpression_ZeroParameters()
    {
        _ = this._emptyConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void CompileModelInstantiatingExpression_OneParameter()
    {
        _ = this._oneParameterConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void CompileModelInstantiatingExpression_TwoParameters()
    {
        _ = this._twoParametersConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void CompileModelInstantiatingExpression_ThreeParameters()
    {
        _ = this._threeParametersConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void CompileModelInstantiatingExpression_FourParameters()
    {
        _ = this._fourParametersConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }
}