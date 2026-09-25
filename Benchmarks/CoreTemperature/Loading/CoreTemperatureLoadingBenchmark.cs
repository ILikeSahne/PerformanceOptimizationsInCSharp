using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
[EventPipeProfiler(EventPipeProfile.GcVerbose)]
public class CoreTemperatureLoadingBenchmark
{
    private string[] _lines = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        _lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
    }

    [Benchmark(Baseline = true)]
    public List<TemperatureReading> Load()
    {
        return TemperatureReadingFile.Load(_lines);
    }

    [Benchmark]
    public List<TemperatureReadingStruct> LoadStructs()
    {
        return CoreTemperatureLoading.LoadStructs(_lines);
    }

    [Benchmark]
    public List<TemperatureReadingStruct> LoadSpanSplit()
    {
        return CoreTemperatureLoading.LoadSpanSplit(_lines);
    }
}
