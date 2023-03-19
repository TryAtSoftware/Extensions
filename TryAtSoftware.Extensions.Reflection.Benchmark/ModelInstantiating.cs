namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using BenchmarkDotNet.Attributes;
using TryAtSoftware.Extensions.Reflection.Benchmark.Models;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2 
|                                         Method |       N |          Mean |        Error |       StdDev |        Median |
|----------------------------------------------- |-------- |--------------:|-------------:|-------------:|--------------:|
| InstantiateModelUsingReflection_ZeroParameters |   10000 |      77.93 us |     0.631 us |     0.590 us |      78.06 us |
| InstantiateModelUsingExpression_ZeroParameters |   10000 |      37.92 us |     0.469 us |     0.416 us |      38.07 us |
|   InstantiateModelUsingReflection_OneParameter |   10000 |   2,582.02 us |    13.390 us |    12.525 us |   2,580.40 us |
|   InstantiateModelUsingExpression_OneParameter |   10000 |     179.05 us |     2.905 us |     2.268 us |     179.30 us |
|  InstantiateModelUsingReflection_TwoParameters |   10000 |   2,934.83 us |    49.102 us |    38.336 us |   2,928.20 us |
|  InstantiateModelUsingExpression_TwoParameters |   10000 |     219.47 us |     1.993 us |     1.766 us |     219.46 us |
| InstantiateModelUsingReflection_ZeroParameters |   50000 |     377.30 us |     4.604 us |     3.594 us |     377.36 us |
| InstantiateModelUsingExpression_ZeroParameters |   50000 |     190.67 us |     2.206 us |     1.956 us |     191.05 us |
|   InstantiateModelUsingReflection_OneParameter |   50000 |  12,781.73 us |    48.905 us |    45.746 us |  12,780.17 us |
|   InstantiateModelUsingExpression_OneParameter |   50000 |     930.04 us |     5.675 us |     5.031 us |     931.25 us |
|  InstantiateModelUsingReflection_TwoParameters |   50000 |  14,296.94 us |    58.386 us |    54.614 us |  14,302.57 us |
|  InstantiateModelUsingExpression_TwoParameters |   50000 |   1,125.47 us |    16.414 us |    14.550 us |   1,125.22 us |
| InstantiateModelUsingReflection_ZeroParameters |  100000 |     750.47 us |     7.472 us |     6.989 us |     751.48 us |
| InstantiateModelUsingExpression_ZeroParameters |  100000 |     377.86 us |     6.750 us |     5.270 us |     378.43 us |
|   InstantiateModelUsingReflection_OneParameter |  100000 |  25,465.00 us |   177.435 us |   148.166 us |  25,494.48 us |
|   InstantiateModelUsingExpression_OneParameter |  100000 |   2,133.85 us |    78.692 us |   224.514 us |   2,071.50 us |
|  InstantiateModelUsingReflection_TwoParameters |  100000 |  30,566.15 us |   607.360 us |   723.018 us |  30,425.55 us |
|  InstantiateModelUsingExpression_TwoParameters |  100000 |   2,455.62 us |    80.495 us |   229.658 us |   2,323.66 us |
| InstantiateModelUsingReflection_ZeroParameters |  500000 |   3,798.05 us |    29.713 us |    24.811 us |   3,792.89 us |
| InstantiateModelUsingExpression_ZeroParameters |  500000 |   1,850.00 us |    10.479 us |     8.182 us |   1,847.42 us |
|   InstantiateModelUsingReflection_OneParameter |  500000 | 128,808.24 us |   539.132 us |   420.919 us | 128,937.91 us |
|   InstantiateModelUsingExpression_OneParameter |  500000 |   9,364.10 us |    51.337 us |    42.869 us |   9,361.82 us |
|  InstantiateModelUsingReflection_TwoParameters |  500000 | 145,380.24 us | 1,098.876 us |   974.125 us | 145,880.02 us |
|  InstantiateModelUsingExpression_TwoParameters |  500000 |  11,338.64 us |    83.900 us |    65.503 us |  11,336.39 us |
| InstantiateModelUsingReflection_ZeroParameters | 1000000 |   7,665.97 us |   148.265 us |   115.756 us |   7,675.05 us |
| InstantiateModelUsingExpression_ZeroParameters | 1000000 |   3,653.81 us |    24.165 us |    20.179 us |   3,652.52 us |
|   InstantiateModelUsingReflection_OneParameter | 1000000 | 262,433.90 us | 1,709.860 us | 1,599.404 us | 262,135.05 us |
|   InstantiateModelUsingExpression_OneParameter | 1000000 |  18,651.48 us |   189.806 us |   158.496 us |  18,671.90 us |
|  InstantiateModelUsingReflection_TwoParameters | 1000000 | 286,330.14 us | 1,480.609 us | 1,384.962 us | 286,620.65 us |
|  InstantiateModelUsingExpression_TwoParameters | 1000000 |  22,372.73 us |   196.658 us |   153.538 us |  22,342.79 us |
 */
public class ModelInstantiating
{
    private Type _type = null!;
    private Func<object?[], BenchmarkModelWithConstructors> _compiledExpressionZeroParameters = null!;
    private Func<object?[], BenchmarkModelWithConstructors> _compiledExpressionOneParameter = null!;
    private Func<object?[], BenchmarkModelWithConstructors> _compiledExpressionTwoParameters = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._type = typeof(BenchmarkModelWithConstructors);

        var emptyConstructorInfo = this._type.GetConstructor(Array.Empty<Type>())!;
        var oneParameterConstructorInfo = this._type.GetConstructor(new[] { typeof(string) })!;
        var twoParametersConstructorInfo = this._type.GetConstructor(new[] { typeof(string), typeof(int) })!;

        this._compiledExpressionZeroParameters = emptyConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
        this._compiledExpressionOneParameter = oneParameterConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
        this._compiledExpressionTwoParameters = twoParametersConstructorInfo.ConstructObjectInitializer<BenchmarkModelWithConstructors>().Compile();
    }

    [Benchmark]
    public void InstantiateModelUsingReflection_ZeroParameters()
    {
        for (var i = 0; i < this.N; i++) _ = Activator.CreateInstance(this._type);
    }

    [Benchmark]
    public void InstantiateModelUsingExpression_ZeroParameters()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpressionZeroParameters(Array.Empty<object?>());
    }


    [Benchmark]
    public void InstantiateModelUsingReflection_OneParameter()
    {
        for (var i = 0; i < this.N; i++) _ = Activator.CreateInstance(this._type, i.ToString());
    }

    [Benchmark]
    public void InstantiateModelUsingExpression_OneParameter()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpressionOneParameter(new object?[] { i.ToString() });
    }


    [Benchmark]
    public void InstantiateModelUsingReflection_TwoParameters()
    {
        for (var i = 0; i < this.N; i++) _ = Activator.CreateInstance(this._type, i.ToString(), i);
    }

    [Benchmark]
    public void InstantiateModelUsingExpression_TwoParameters()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpressionTwoParameters(new object?[] { i.ToString(), i });
    }
}