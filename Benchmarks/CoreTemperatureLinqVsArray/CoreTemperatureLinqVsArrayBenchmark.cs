using BenchmarkDotNet.Attributes;
using Benchmarks.Data;

namespace Benchmarks.CoreTemperatureLinqVsArray;

[MemoryDiagnoser]
public class CoreTemperatureLinqVsArrayBenchmark
{
    private TemperatureReading[] _readings = null!;
    private Dictionary<Phase, double[]> _result = null!;

    public int ReadingCount { get; set; } = 10_000_000;

    [GlobalSetup]
    public void Setup()
    {
        _readings = TemperatureReadingFaker.Generate(ReadingCount);
        _result = CoreTemperatureResult.Create();
    }

    [Benchmark(Baseline = true)]
    public void Linq() => CoreTemperatureLinqVsArray.Linq(_readings, _result);

    [Benchmark]
    public void Array() => CoreTemperatureLinqVsArray.Array(_readings, _result);
}

public static class CoreTemperatureLinqVsArray
{
    public static void Linq(TemperatureReading[] readings, Dictionary<Phase, double[]> result)
    {
        foreach (var group in readings.GroupBy(r => (r.Phase, r.Core)))
        {
            result[group.Key.Phase][group.Key.Core] = group.Sum(r => r.Celsius);
        }
    }

    public static void Array(TemperatureReading[] readings, Dictionary<Phase, double[]> result)
    {
        var sums = new double[Cpu.PhaseCount * Cpu.CoreCount];

        foreach (var reading in readings)
        {
            sums[(int)reading.Phase * Cpu.CoreCount + reading.Core] += reading.Celsius;
        }

        CoreTemperatureResult.CopyFrom(sums, result);
    }
}
