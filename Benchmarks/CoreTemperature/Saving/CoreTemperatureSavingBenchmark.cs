using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureSavingBenchmark
{
    private List<TemperatureReadingStruct> _spikes = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        var readings = TemperatureReadingFaker.GenerateStructs(ReadingCount);

        var averages = readings
            .GroupBy(r => r.Phase)
            .ToDictionary(g => g.Key, g => g.Average(r => r.Celsius));

        _spikes = readings
            .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
            .ToList();
    }

    [Benchmark(Baseline = true)]
    public string Concat()
    {
        return CoreTemperatureSaving.Concat(_spikes);
    }

    [Benchmark]
    public string Builder()
    {
        return CoreTemperatureSaving.Builder(_spikes);
    }

    [Benchmark]
    public string BuilderExactSize()
    {
        return CoreTemperatureSaving.BuilderExactSize(_spikes);
    }

    [Benchmark]
    public string StringCreate()
    {
        return CoreTemperatureSaving.StringCreate(_spikes);
    }
}
