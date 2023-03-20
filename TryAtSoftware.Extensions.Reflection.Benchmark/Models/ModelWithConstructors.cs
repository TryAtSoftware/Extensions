namespace TryAtSoftware.Extensions.Reflection.Benchmark.Models;

public class ModelWithConstructors
{
    public ModelWithConstructors()
    {
    }

    public ModelWithConstructors(string text1)
    {
        this.Text1 = text1;
    }

    public ModelWithConstructors(string text1, string text2)
        : this(text1)
    {
        this.Text2 = text2;
    }

    public ModelWithConstructors(int number1, int number2 = 13)
    {
        this.Number1 = number1;
        this.Number2 = number2;
    }

    public string? Text1 { get; }
    public string? Text2 { get; }
    
    public int? Number1 { get; }
    public int? Number2 { get; }
}