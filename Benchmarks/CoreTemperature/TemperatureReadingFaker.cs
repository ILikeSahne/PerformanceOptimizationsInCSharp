using Bogus;

namespace Benchmarks.CoreTemperature;

public static class TemperatureReadingFaker
{
    private static readonly double[] BaseCelsiusPerPhase = [45, 55, 80, 65, 40, 50];

    public static TemperatureReading[] Generate(int count)
    {
        return new Faker<TemperatureReading>()
            .UseSeed(42)
            .CustomInstantiator(f =>
            {
                var phase = f.PickRandom<Phase>();

                // Normal noise of +-5 °C, and about 1 in 1000 readings is a spike of +15 to +25 °C
                var celsius = BaseCelsiusPerPhase[(int)phase] + f.Random.Double(-5, 5);

                if (f.Random.Bool(0.001f))
                {
                    celsius += f.Random.Double(15, 25);
                }

                return new TemperatureReading(phase, celsius);
            })
            .Generate(count)
            .ToArray();
    }

    // Faker<T> only supports classes, so we copy the same readings into structs
    public static TemperatureReadingStruct[] GenerateStructs(int count)
    {
        return Generate(count)
            .Select(r => new TemperatureReadingStruct(r.Phase, r.Celsius))
            .ToArray();
    }
}
