using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureFableBenchmark
{
    private List<TemperatureReading> _readings = null!;
    private TemperatureReadingStruct[] _readingStructs = null!;

    public int ReadingCount { get; set; } = 10_000_000;

    [GlobalSetup]
    public void Setup()
    {
        _readings = TemperatureReadingFaker.Generate(ReadingCount).ToList();
        _readingStructs = TemperatureReadingFaker.GenerateStructs(ReadingCount);
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Linq() => CoreTemperatureDictionaryVsArray.Linq(_readings);

    [Benchmark]
    public Dictionary<Phase, int> Struct() => CoreTemperatureClassVsStruct.Struct(_readingStructs);

    // TODO: add Fable's answers for the spikes version here
}

public static class CoreTemperatureFable
{
}
