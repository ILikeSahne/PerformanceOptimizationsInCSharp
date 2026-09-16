using Bogus;

namespace Benchmarks.Data;

public static class TemperatureReadingFaker
{
    public static TemperatureReading[] Generate(int count)
    {
        return new Faker<TemperatureReading>()
            .UseSeed(42)
            .CustomInstantiator(f => new TemperatureReading(
                f.PickRandom<Phase>(),
                f.Random.Int(0, Cpu.CoreCount - 1),
                f.Random.Double(40, 90)))
            .Generate(count)
            .ToArray();
    }

    // Faker<T> only supports classes, so we copy the same readings into structs
    public static TemperatureReadingStruct[] GenerateStructs(int count)
    {
        return Generate(count)
            .Select(r => new TemperatureReadingStruct(r.Phase, r.Core, r.Celsius))
            .ToArray();
    }
}
