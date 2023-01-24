namespace TryAtSoftware.Extensions.Reflection.Tests.Randomization;

using TryAtSoftware.Extensions.Reflection.Tests.Models;
using TryAtSoftware.Randomizer.Core;
using TryAtSoftware.Randomizer.Core.Primitives;

public class StudentRandomizer : ComplexRandomizer<Student>
{
    public StudentRandomizer()
        : base(new GeneralInstanceBuilder<Student>())
    {
        this.AddRandomizationRule(x => x.FirstName, new StringRandomizer());
        this.AddRandomizationRule(x => x.MiddleName, new StringRandomizer());
        this.AddRandomizationRule(x => x.LastName, new StringRandomizer());
        this.AddRandomizationRule(x => x.Age, new NumberRandomizer());

        this.AddRandomizationRule(x => x.School, new StringRandomizer());
    }
}