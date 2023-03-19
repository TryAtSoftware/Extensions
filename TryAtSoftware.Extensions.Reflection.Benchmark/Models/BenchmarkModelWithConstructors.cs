namespace TryAtSoftware.Extensions.Reflection.Benchmark.Models;

public class BenchmarkModelWithConstructors
{
    public BenchmarkModelWithConstructors()
    {
    }

    public BenchmarkModelWithConstructors(string text)
    {
        this.Text = text;
    }

    public BenchmarkModelWithConstructors(string text, int number)
        : this(text)
    {
        this.Number = number;
    }

    public BenchmarkModelWithConstructors(string text, int number, char symbol)
        : this(text, number)
    {
        this.Symbol = symbol;
    }

    public BenchmarkModelWithConstructors(string text, int number, char symbol, bool truth)
        : this(text, number, symbol)
    {
        this.Truth = truth;
    }

    public string? Text { get; }
    public int? Number { get; }
    public char? Symbol { get; }
    public bool? Truth { get; }
}