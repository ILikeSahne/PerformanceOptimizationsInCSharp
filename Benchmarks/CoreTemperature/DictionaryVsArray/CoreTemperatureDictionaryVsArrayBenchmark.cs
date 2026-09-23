using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureDictionaryVsArrayBenchmark
{
    private string[] _lines = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        _lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Linq()
    {
        var readings = TemperatureReadingFile.LoadList(_lines);
        return CoreTemperatureDictionaryVsArray.Linq(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> Array()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureDictionaryVsArray.Array(readings);
    }
}

public static class CoreTemperatureDictionaryVsArray
{
    public static Dictionary<Phase, int> Linq(List<TemperatureReading> readings)
    {
        var averages = readings
            .GroupBy(r => r.Phase)
            .ToDictionary(g => g.Key, g => g.Average(r => r.Celsius));

        return readings
            .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
            .GroupBy(r => r.Phase)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public static Dictionary<Phase, int> Array(TemperatureReading[] readings)
    {
        var sums = new double[Cpu.PhaseCount];
        var counts = new int[Cpu.PhaseCount];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;
            sums[phase] += reading.Celsius;
            counts[phase]++;
        }

        var spikes = new int[Cpu.PhaseCount];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;

            if (reading.Celsius > sums[phase] / counts[phase] + Spike.Threshold)
            {
                spikes[phase]++;
            }
        }

        return PhaseDictionary.From(spikes);
    }
}
