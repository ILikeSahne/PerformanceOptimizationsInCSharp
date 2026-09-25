using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
public class CoreTemperatureLoadingAndSpikesBenchmark
{
    private string[] _lines = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        _lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> LoadLinq()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureLoadingAndSpikes.Linq(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> LoadNoLinqArrayStaticCount()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureLoadingAndSpikes.NoLinqArrayStaticCount(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> LoadSpanSplitNoLinqArrayStaticCount()
    {
        var readings = CoreTemperatureLoadingAndSpikes.LoadSpanSplit(_lines);
        return CoreTemperatureLoadingAndSpikes.NoLinqArrayStaticCount(readings);
    }
}
