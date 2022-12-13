namespace TryAtSoftware.Extensions.Reflection.Tests.Randomization;

using TryAtSoftware.Extensions.Reflection.Tests.Types;
using TryAtSoftware.Randomizer.Core;
using TryAtSoftware.Randomizer.Core.Primitives;

public class PersonRandomizer : ComplexRandomizer<Person>
{
    public PersonRandomizer()
        : base(new GeneralInstanceBuilder<Person>())
    {
        this.AddRandomizationRule(x => x.FirstName, new StringRandomizer());
        this.AddRandomizationRule(x => x.MiddleName, new StringRandomizer());
        this.AddRandomizationRule(x => x.LastName, new StringRandomizer());
        this.AddRandomizationRule(x => x.Age, new NumberRandomizer());
    }
}