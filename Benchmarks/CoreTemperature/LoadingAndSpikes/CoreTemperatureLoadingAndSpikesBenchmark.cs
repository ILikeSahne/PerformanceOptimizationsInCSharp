using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

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
    public Dictionary<Phase, int> LoadNoLinqArray()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureLoadingAndSpikes.NoLinqArray(readings);
    }

    /*[Benchmark]
    public Dictionary<Phase, int> LoadNoLinqSpan()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureLoadingAndSpikes.NoLinqSpan(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> LoadSpanSplitNoLinqSpan()
    {
        var readings = CoreTemperatureLoadingAndSpikes.LoadSpanSplit(_lines);
        return CoreTemperatureLoadingAndSpikes.NoLinqSpan(readings);
    }*/
}
