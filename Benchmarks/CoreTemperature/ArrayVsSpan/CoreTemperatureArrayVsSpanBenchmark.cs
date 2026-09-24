using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureArrayVsSpanBenchmark
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
    public Dictionary<Phase, int> NoLinqArray()
    {
        return CoreTemperatureArrayVsSpan.NoLinqArray(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqSpan()
    {
        return CoreTemperatureArrayVsSpan.NoLinqSpan(_readings);
    }
}
