namespace Benchmarks.CoreTemperature;

public enum Phase
{
    Startup = 0,
    Import = 1,
    Processing = 2,
    Export = 3,
    Idle = 4,
    Shutdown = 5,
}

public static class Phases
{
    public static readonly int Count = Enum.GetValues<Phase>().Length;
}

public record TemperatureReading(Phase Phase, double Celsius);

public readonly record struct TemperatureReadingStruct(Phase Phase, double Celsius);

public static class Spike
{
    public const double Threshold = 10;
}
