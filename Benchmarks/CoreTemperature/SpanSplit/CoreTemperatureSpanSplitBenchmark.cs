using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System.Globalization;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
[EventPipeProfiler(EventPipeProfile.GcVerbose)]
public class CoreTemperatureSpanSplitBenchmark
{
    private string[] _lines = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        _lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Split()
    {
        var readings = TemperatureReadingFile.LoadStructs(_lines);
        return CoreTemperatureClassVsStruct.Struct(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> SpanSplit()
    {
        var readings = CoreTemperatureSpanSplit.LoadStructs(_lines);
        return CoreTemperatureClassVsStruct.Struct(readings);
    }
}

public static class CoreTemperatureSpanSplit
{
    public static TemperatureReadingStruct[] LoadStructs(string[] lines)
    {
        var readings = new List<TemperatureReadingStruct>();
        Span<Range> parts = stackalloc Range[2];

        foreach (var line in lines)
        {
            var span = line.AsSpan();
            span.Split(parts, ';');

            readings.Add(new TemperatureReadingStruct(
                Enum.Parse<Phase>(span[parts[0]]),
                double.Parse(span[parts[1]], CultureInfo.InvariantCulture)));
        }

        return readings.ToArray();
    }
}
