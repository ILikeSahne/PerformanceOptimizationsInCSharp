using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureDictionaryLookupsBenchmark
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
    public Dictionary<Phase, int> NoLinq()
    {
        return CoreTemperatureDictionaryLookups.NoLinq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> FastLinq()
    {
        return CoreTemperatureDictionaryLookups.FastLinq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqValueRef()
    {
        return CoreTemperatureDictionaryLookups.NoLinqValueRef(_readings);
    }
}
