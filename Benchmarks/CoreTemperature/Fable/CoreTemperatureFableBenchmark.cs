using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureFableBenchmark
{
    private List<TemperatureReading> _readings = null!;
    private List<TemperatureReadingStruct> _readingStructs = null!;

    public int ReadingCount { get; set; } = 10_000_000;

    [GlobalSetup]
    public void Setup()
    {
        _readings = TemperatureReadingFaker.Generate(ReadingCount).ToList();
        _readingStructs = TemperatureReadingFaker.GenerateStructs(ReadingCount).ToList();
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Linq() => CoreTemperatureFable.Linq(_readings);

    [Benchmark]
    public Dictionary<Phase, int> Struct() => CoreTemperatureFable.NoLinqSpan(_readingStructs);
}
