namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using BenchmarkDotNet.Attributes;

/*
16/03/2023, TryAtSoftware.Extensions.Reflection v1.1.1-alpha.2 
|                          Method |       N |        Mean |     Error |    StdDev |
|-------------------------------- |-------- |------------:|----------:|----------:|
| InstantiateModelUsingReflection |   10000 |    64.61 us |  1.289 us |  1.806 us |
| InstantiateModelUsingExpression |   10000 |    34.33 us |  0.407 us |  0.339 us |
| InstantiateModelUsingReflection |   50000 |   331.90 us |  5.148 us |  4.564 us |
| InstantiateModelUsingExpression |   50000 |   175.44 us |  1.755 us |  1.555 us |
| InstantiateModelUsingReflection |  100000 |   661.81 us |  3.951 us |  3.696 us |
| InstantiateModelUsingExpression |  100000 |   349.29 us |  4.174 us |  3.700 us |
| InstantiateModelUsingReflection |  500000 | 3,218.84 us | 25.818 us | 21.559 us |
| InstantiateModelUsingExpression |  500000 | 1,751.27 us | 25.142 us | 20.995 us |
| InstantiateModelUsingReflection | 1000000 | 6,604.77 us | 36.223 us | 33.883 us |
| InstantiateModelUsingExpression | 1000000 | 3,392.57 us | 45.462 us | 40.301 us |
 */
public class ModelInstantiating
{
    private Type _type = null!;
    private Func<object?[], BenchmarkModel> _compiledExpression = null!;

    [Params(10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._type = typeof(BenchmarkModel);
        var constructorInfo = this._type.GetConstructors().Single();

        var instanceBuildingExpression = constructorInfo.ConstructObjectInitializer<BenchmarkModel>();
        this._compiledExpression = instanceBuildingExpression.Compile();
    }

    [Benchmark]
    public void InstantiateModelUsingReflection()
    {
        for (var i = 0; i < this.N; i++) _ = Activator.CreateInstance(this._type);
    }

    [Benchmark]
    public void InstantiateModelUsingExpression()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpression(Array.Empty<object?>());
    }
}