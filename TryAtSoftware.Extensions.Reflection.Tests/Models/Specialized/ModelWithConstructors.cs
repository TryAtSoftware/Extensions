namespace TryAtSoftware.Extensions.Reflection.Tests.Models.Specialized;

public class ModelWithConstructors
{
    public const char DefaultSymbol = 't';
    public const string DefaultText = "Hello, world!";
    public const int DefaultNumber = 13;

    private ModelWithConstructors(int constructorIndex, string text, int number)
    {
        this.UsedConstructor = constructorIndex;
        this.Text = text;
        this.Number = number;
    }

    public ModelWithConstructors(string text, int number, char symbol = DefaultSymbol) : this(1, text, number)
    {
        this.Symbol = symbol;
    }

    public ModelWithConstructors(string text) : this(2, text, DefaultNumber)
    {
    }

    public ModelWithConstructors(int number) : this(3, DefaultText, number)
    {
    }

    public ModelWithConstructors() : this(4, DefaultText, DefaultNumber)
    {
    }

    public int UsedConstructor { get; }
    public string Text { get; }
    public char Symbol { get; }
    public int Number { get; }
}
