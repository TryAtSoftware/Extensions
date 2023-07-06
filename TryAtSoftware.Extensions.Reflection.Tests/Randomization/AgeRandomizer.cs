namespace TryAtSoftware.Extensions.Reflection.Tests.Randomization;

using TryAtSoftware.Randomizer.Core.Helpers;
using TryAtSoftware.Randomizer.Core.Interfaces;

public class AgeRandomizer : IRandomizer<ushort>
{
    public ushort PrepareRandomValue() => (ushort)RandomizationHelper.RandomInteger(0, 1000);
}