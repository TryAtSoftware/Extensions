namespace TryAtSoftware.Extensions.Reflection.Benchmark;

using BenchmarkDotNet.Attributes;

public class ModelInstantiating
{
    private Type _type = null!;
    private Func<object?[], BenchmarkModel> _compiledExpression = null!;

    [Params(100, 1_000, 5_000, 10_000, 50_000, 100_000, 500_000, 1_000_000)] public int N { get; set; }

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
        for (var i = 0; i < this.N; i++) _ = Activator.CreateInstance(typeof(BenchmarkModel));
    }

    [Benchmark]
    public void InstantiateModelUsingExpression()
    {
        for (var i = 0; i < this.N; i++) _ = this._compiledExpression(Array.Empty<object?>());
    }
}