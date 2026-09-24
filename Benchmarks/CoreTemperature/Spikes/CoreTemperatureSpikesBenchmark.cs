using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureSpikesBenchmark
{
    private List<TemperatureReading> _readings = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        var lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
        _readings = TemperatureReadingFile.Load(lines);
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Linq()
    {
        return CoreTemperatureSpikes.Linq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> FastLinq()
    {
        return CoreTemperatureSpikes.FastLinq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqArray()
    {
        return CoreTemperatureSpikes.NoLinqArray(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqSpan()
    {
        return CoreTemperatureSpikes.NoLinqSpan(_readings);
    }
}
