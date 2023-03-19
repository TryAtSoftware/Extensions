using BenchmarkDotNet.Running;
using TryAtSoftware.Extensions.Reflection.Benchmark;

BenchmarkRunner.Run<ValueRetrieving>();
BenchmarkRunner.Run<ValueSetting>();
BenchmarkRunner.Run<ModelInstantiating>();
BenchmarkRunner.Run<ExpressionsCompiling>();