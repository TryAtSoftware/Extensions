namespace TryAtSoftware.Extensions.Reflection.Tests.Models.Specialized;

public class ModelWithConstructors
{
    private ModelWithConstructors(int constructorIndex, string text, int number)
    {
        this.UsedConstructor = constructorIndex;
        this.Text = text;
        this.Number = number;
    }

    public ModelWithConstructors(string text, int number, char symbol = 't') : this(1, text, number)
    {
        this.Symbol = symbol;
    }

    public ModelWithConstructors(string text) : this(2, text, 13)
    {
    }

    public ModelWithConstructors(int number) : this(3, "Hello, world", number)
    {
    }

    public ModelWithConstructors() : this(4, "Hello, world", 13)
    {
    }

    public int UsedConstructor { get; }
    public string Text { get; }
    public char Symbol { get; }
    public int Number { get; }
}
