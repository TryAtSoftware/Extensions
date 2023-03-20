namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
Examined on 20/03/2023:
|                                                                Method |           Mean |         Error |        StdDev |
|---------------------------------------------------------------------- |---------------:|--------------:|--------------:|
|                        InstantiateModelUsingReflection_ZeroParameters |       8.396 ns |     0.0641 ns |     0.0599 ns |
|             InstantiateModelUsingConstructorInvocation_ZeroParameters |      12.318 ns |     0.1068 ns |     0.0999 ns |
|                        InstantiateModelUsingExpression_ZeroParameters |       4.960 ns |     0.1138 ns |     0.1310 ns |
|                          InstantiateModelUsingReflection_OneParameter |     275.653 ns |     2.4048 ns |     1.8775 ns |
|               InstantiateModelUsingConstructorInvocation_OneParameter |      30.740 ns |     0.1956 ns |     0.1633 ns |
|                          InstantiateModelUsingExpression_OneParameter |      10.737 ns |     0.2087 ns |     0.1743 ns |
|                         InstantiateModelUsingReflection_TwoParameters |     349.846 ns |     2.0746 ns |     1.8391 ns |
|              InstantiateModelUsingConstructorInvocation_TwoParameters |      42.796 ns |     0.3557 ns |     0.2970 ns |
|                         InstantiateModelUsingExpression_TwoParameters |      13.665 ns |     0.0696 ns |     0.0774 ns |
|                    InstantiateModelUsingReflection_OptionalParameters |     445.106 ns |     6.2165 ns |     5.5108 ns |
|         InstantiateModelUsingConstructorInvocation_OptionalParameters |     148.657 ns |     1.3886 ns |     1.2310 ns |
|                    InstantiateModelUsingExpression_OptionalParameters |      17.780 ns |     0.2495 ns |     0.3415 ns |
|                    CompileModelInstantiatingExpression_ZeroParameters |  46,628.233 ns |   924.4086 ns | 1,713.4506 ns |
|     CompileModelInstantiatingExpression_ZeroParameters_WithValidation |  92,992.725 ns |   457.8785 ns |   405.8974 ns |
|                      CompileModelInstantiatingExpression_OneParameter | 109,652.003 ns |   714.1525 ns |   633.0775 ns |
|       CompileModelInstantiatingExpression_OneParameter_WithValidation | 160,086.966 ns | 3,105.9800 ns | 3,050.4877 ns |
|                     CompileModelInstantiatingExpression_TwoParameters | 157,889.117 ns | 1,071.3123 ns |   949.6904 ns |
|      CompileModelInstantiatingExpression_TwoParameters_WithValidation | 200,620.907 ns |   836.0908 ns |   741.1727 ns |
|                CompileModelInstantiatingExpression_OptionalParameters | 163,244.454 ns | 1,572.9164 ns | 1,228.0297 ns |
| CompileModelInstantiatingExpression_OptionalParameters_WithValidation | 211,173.854 ns | 1,225.5750 ns | 1,086.4404 ns |
 */
public class ModelInstantiating
{
    private Type _type = null!;
    
    private ConstructorInfo _emptyConstructorInfo = null!;
    private ConstructorInfo _oneParameterConstructorInfo = null!;
    private ConstructorInfo _twoParametersConstructorInfo = null!;
    private ConstructorInfo _optionalParametersConstructorInfo = null!;
    
    private Func<object?[], ModelWithConstructors> _compiledExpressionZeroParameters = null!;
    private Func<object?[], ModelWithConstructors> _compiledExpressionOneParameter = null!;
    private Func<object?[], ModelWithConstructors> _compiledExpressionTwoParameters = null!;
    private Func<object?[], ModelWithConstructors> _compiledExpressionOptionalParameters = null!;

    [GlobalSetup]
    public void Setup()
    {
        this._type = typeof(ModelWithConstructors);

        this._emptyConstructorInfo = this._type.GetConstructor(Array.Empty<Type>())!;
        this._oneParameterConstructorInfo = this._type.GetConstructor(new[] { typeof(string) })!;
        this._twoParametersConstructorInfo = this._type.GetConstructor(new[] { typeof(string), typeof(string) })!;
        this._optionalParametersConstructorInfo = this._type.GetConstructor(new[] { typeof(int), typeof(int) })!;

        this._compiledExpressionZeroParameters = this._emptyConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
        this._compiledExpressionOneParameter = this._oneParameterConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
        this._compiledExpressionTwoParameters = this._twoParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
        this._compiledExpressionOptionalParameters = this._optionalParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void InstantiateModelUsingReflection_ZeroParameters() => _ = Activator.CreateInstance(this._type);
    
    [Benchmark]
    public void InstantiateModelUsingConstructorInvocation_ZeroParameters() => _ = this._emptyConstructorInfo.Invoke(Array.Empty<object?>());
    
    [Benchmark]
    public void InstantiateModelUsingExpression_ZeroParameters() => _ = this._compiledExpressionZeroParameters(Array.Empty<object?>());
    
    [Benchmark]
    public void InstantiateModelUsingReflection_OneParameter() => _ = Activator.CreateInstance(this._type, "value1");
    
    [Benchmark]
    public void InstantiateModelUsingConstructorInvocation_OneParameter() => _ = this._oneParameterConstructorInfo.Invoke(new object[] { "value1" });
    
    [Benchmark]
    public void InstantiateModelUsingExpression_OneParameter() => _ = this._compiledExpressionOneParameter(new object?[] { "value1" });
    
    [Benchmark]
    public void InstantiateModelUsingReflection_TwoParameters() => _ = Activator.CreateInstance(this._type, "value1", "value2");
    
    [Benchmark]
    public void InstantiateModelUsingConstructorInvocation_TwoParameters() => _ = this._twoParametersConstructorInfo.Invoke(new object[] { "value1", "value2" });
    
    [Benchmark]
    public void InstantiateModelUsingExpression_TwoParameters() => _ = this._compiledExpressionTwoParameters(new object?[] { "value1", "value2" });
    
    [Benchmark]
    public void InstantiateModelUsingReflection_OptionalParameters() => _ = Activator.CreateInstance(this._type, BindingFlags.OptionalParamBinding, null, new object?[] { 25 }, CultureInfo.InvariantCulture);
    
    [Benchmark]
    public void InstantiateModelUsingConstructorInvocation_OptionalParameters() => _ = this._optionalParametersConstructorInfo.Invoke(new[] { 25, Type.Missing });
    
    [Benchmark]
    public void InstantiateModelUsingExpression_OptionalParameters() => _ = this._compiledExpressionOptionalParameters(new object?[] { 25, null });
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_ZeroParameters() => _ = this._emptyConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_ZeroParameters_WithValidation() => _ = this._emptyConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true).Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_OneParameter() => _ = this._oneParameterConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();

    [Benchmark]
    public void CompileModelInstantiatingExpression_OneParameter_WithValidation() => _ = this._oneParameterConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true).Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_TwoParameters() => _ = this._twoParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();

    [Benchmark]
    public void CompileModelInstantiatingExpression_TwoParameters_WithValidation() => _ = this._twoParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true).Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_OptionalParameters() => _ = this._optionalParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();

    [Benchmark]
    public void CompileModelInstantiatingExpression_OptionalParameters_WithValidation() => _ = this._optionalParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>(includeParametersCountValidation: true).Compile();
}