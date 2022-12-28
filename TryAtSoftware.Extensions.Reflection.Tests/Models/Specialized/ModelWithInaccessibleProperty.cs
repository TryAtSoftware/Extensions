namespace TryAtSoftware.Extensions.Reflection.Tests.Models.Specialized;

public class ModelWithInaccessibleProperty
{
    private string? InaccessibleProperty { get; set; }
    
#pragma warning disable S2376
    public string InaccessiblePropertySetter { set => this.InaccessibleProperty = value; }
#pragma warning restore S2376
    
    public string? InaccessiblePropertyGetter { get => this.InaccessibleProperty; }
}