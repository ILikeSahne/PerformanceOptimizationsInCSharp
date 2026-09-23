using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace Benchmarks.CoreTemperature;

[MemoryDiagnoser]
[EventPipeProfiler(EventPipeProfile.GcVerbose)]
public class CoreTemperatureClassVsStructBenchmark
{
    private string[] _lines = null!;

    public int ReadingCount { get; set; } = 100_000;

    [GlobalSetup]
    public void Setup()
    {
        _lines = TemperatureReadingFile.ToLines(TemperatureReadingFaker.Generate(ReadingCount));
    }

    [Benchmark(Baseline = true)]
    public Dictionary<Phase, int> Class()
    {
        var readings = TemperatureReadingFile.Load(_lines);
        return CoreTemperatureDictionaryVsArray.Array(readings);
    }

    [Benchmark]
    public Dictionary<Phase, int> Struct()
    {
        var readings = TemperatureReadingFile.LoadStructs(_lines);
        return CoreTemperatureClassVsStruct.Struct(readings);
    }
}

public static class CoreTemperatureClassVsStruct
{
    public static Dictionary<Phase, int> Struct(TemperatureReadingStruct[] readings)
    {
        var sums = new double[Cpu.PhaseCount];
        var counts = new int[Cpu.PhaseCount];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;
            sums[phase] += reading.Celsius;
            counts[phase]++;
        }

        var spikes = new int[Cpu.PhaseCount];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;

            if (reading.Celsius > sums[phase] / counts[phase] + Spike.Threshold)
            {
                spikes[phase]++;
            }
        }

        return PhaseDictionary.From(spikes);
    }
}
