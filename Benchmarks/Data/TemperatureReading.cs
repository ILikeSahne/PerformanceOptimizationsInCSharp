namespace Benchmarks.Data;

public enum Phase
{
    Startup = 0,
    Import = 1,
    Processing = 2,
    Export = 3,
    Idle = 4,
    Shutdown = 5,
}

public class TemperatureReading(Phase phase, int core, double celsius)
{
    public Phase Phase { get; } = phase;
    public int Core { get; } = core;
    public double Celsius { get; } = celsius;
}

public readonly struct TemperatureReadingStruct(Phase phase, int core, double celsius)
{
    public Phase Phase { get; } = phase;
    public int Core { get; } = core;
    public double Celsius { get; } = celsius;
}

public static class Cpu
{
    public const int CoreCount = 16;

    public const int PhaseCount = (int)Phase.Shutdown + 1;
}
