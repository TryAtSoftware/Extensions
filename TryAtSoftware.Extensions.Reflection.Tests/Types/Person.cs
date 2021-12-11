namespace TryAtSoftware.Extensions.Reflection.Tests.Types;

public class Person
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public ushort Age { get; set; }

    public string FullName => $"{this.FirstName} {this.MiddleName} {this.LastName}";
}