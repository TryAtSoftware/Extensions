namespace TryAtSoftware.Extensions.Reflection.Tests.Randomization;

using TryAtSoftware.Extensions.Reflection.Tests.Models;
using TryAtSoftware.Randomizer.Core;
using TryAtSoftware.Randomizer.Core.Primitives;

public class StudentRandomizer : ComplexRandomizer<Student>
{
    public StudentRandomizer()
        : base(new GeneralInstanceBuilder<Student>())
    {
        this.Randomize(x => x.FirstName, new StringRandomizer());
        this.Randomize(x => x.MiddleName, new StringRandomizer());
        this.Randomize(x => x.LastName, new StringRandomizer());
        this.Randomize(x => x.Age, new AgeRandomizer());

        this.Randomize(x => x.School, new StringRandomizer());
    }
}