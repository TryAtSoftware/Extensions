namespace TryAtSoftware.Extensions.Reflection.Tests.Models.Specialized;

public class ModelWithInaccessibleProperty
{
    private string InaccessibleProperty { get; set; }
    public string InaccessiblePropertySetter { set => this.InaccessibleProperty = value; }
    public string InaccessiblePropertyGetter { get => this.InaccessibleProperty; }
}