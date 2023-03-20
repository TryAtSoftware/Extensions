namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2
|                                                        Method |           Mean |         Error |        StdDev |         Median |
|-------------------------------------------------------------- |---------------:|--------------:|--------------:|---------------:|
|                InstantiateModelUsingReflection_ZeroParameters |       7.879 ns |     0.1447 ns |     0.2457 ns |       7.785 ns |
|     InstantiateModelUsingConstructorInvocation_ZeroParameters |      11.843 ns |     0.2483 ns |     0.2550 ns |      11.782 ns |
|                InstantiateModelUsingExpression_ZeroParameters |       4.364 ns |     0.1107 ns |     0.0925 ns |       4.389 ns |
|                  InstantiateModelUsingReflection_OneParameter |     282.762 ns |     2.6890 ns |     2.5153 ns |     281.976 ns |
|       InstantiateModelUsingConstructorInvocation_OneParameter |      31.854 ns |     0.2600 ns |     0.2432 ns |      31.803 ns |
|                  InstantiateModelUsingExpression_OneParameter |      10.366 ns |     0.1512 ns |     0.1680 ns |      10.370 ns |
|                 InstantiateModelUsingReflection_TwoParameters |     363.171 ns |     4.3010 ns |     3.5915 ns |     362.873 ns |
|      InstantiateModelUsingConstructorInvocation_TwoParameters |      42.800 ns |     0.7584 ns |     0.6723 ns |      42.575 ns |
|                 InstantiateModelUsingExpression_TwoParameters |      12.902 ns |     0.1436 ns |     0.1596 ns |      12.950 ns |
|            InstantiateModelUsingReflection_OptionalParameters |     480.628 ns |     9.5506 ns |    17.7026 ns |     474.349 ns |
| InstantiateModelUsingConstructorInvocation_OptionalParameters |     130.349 ns |     0.8773 ns |     0.8206 ns |     130.368 ns |
|            InstantiateModelUsingExpression_OptionalParameters |      19.324 ns |     0.3094 ns |     0.2894 ns |      19.382 ns |
|            CompileModelInstantiatingExpression_ZeroParameters |  47,160.173 ns |   873.2294 ns | 1,005.6123 ns |  46,780.139 ns |
|              CompileModelInstantiatingExpression_OneParameter | 113,940.085 ns | 2,175.4359 ns | 4,344.5806 ns | 111,696.851 ns |
|             CompileModelInstantiatingExpression_TwoParameters | 161,824.582 ns |   859.5220 ns |   803.9974 ns | 161,836.035 ns |
|        CompileModelInstantiatingExpression_OptionalParameters | 166,530.435 ns | 1,653.4656 ns | 1,380.7186 ns | 167,216.040 ns |
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
    public void InstantiateModelUsingReflection_ThreeParameters() => _ = Activator.CreateInstance(this._type, "value1", "value2", "value3");
    
    [Benchmark]
    public void InstantiateModelUsingReflection_OptionalParameters() => _ = Activator.CreateInstance(this._type, BindingFlags.OptionalParamBinding, null, new object?[] { 25 }, CultureInfo.InvariantCulture);

    [Benchmark]
    public void InstantiateModelUsingConstructorInvocation_OptionalParameters() => _ = this._optionalParametersConstructorInfo.Invoke(new[] { 25, Type.Missing });
    
    [Benchmark]
    public void InstantiateModelUsingExpression_OptionalParameters() => _ = this._compiledExpressionOptionalParameters(new object?[] { 25, null });
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_ZeroParameters() => _ = this._emptyConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_OneParameter() => _ = this._oneParameterConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_TwoParameters() => _ = this._twoParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
    
    [Benchmark]
    public void CompileModelInstantiatingExpression_OptionalParameters() => _ = this._optionalParametersConstructorInfo.ConstructObjectInitializer<ModelWithConstructors>().Compile();
}