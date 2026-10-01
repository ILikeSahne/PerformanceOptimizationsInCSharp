using BenchmarkDotNet.Attributes;

namespace Benchmarks.CoreTemperature;

// All steps of the processing part in one run, so the first and the final version can be compared directly
[MemoryDiagnoser]
public class CoreTemperatureProcessingBenchmark
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
        return CoreTemperatureLinqVsFastLinq.Linq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinq()
    {
        return CoreTemperatureLinqVsFastLinq.NoLinq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqValueRef()
    {
        return CoreTemperatureDictionaryLookups.NoLinqValueRef(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> FastLinq()
    {
        return CoreTemperatureLinqVsFastLinq.FastLinq(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqArray()
    {
        return CoreTemperatureDictionaryVsArray.NoLinqArray(_readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> NoLinqArrayStaticCount()
    {
        return CoreTemperatureDictionaryVsArray.NoLinqArrayStaticCount(_readings);
    }
}
