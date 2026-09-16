using BenchmarkDotNet.Attributes;
using Benchmarks.Data;

namespace Benchmarks.CoreTemperatureClassVsStruct;

[MemoryDiagnoser]
public class CoreTemperatureClassVsStructBenchmark
{
    private TemperatureReading[] _readings = null!;
    private TemperatureReadingStruct[] _readingStructs = null!;
    private Dictionary<Phase, double[]> _result = null!;

    public int ReadingCount { get; set; } = 10_000_000;

    [GlobalSetup]
    public void Setup()
    {
        _readings = TemperatureReadingFaker.Generate(ReadingCount);
        _readingStructs = TemperatureReadingFaker.GenerateStructs(ReadingCount);
        _result = CoreTemperatureResult.Create();
    }

    [Benchmark(Baseline = true)]
    public void Class() => CoreTemperatureClassVsStruct.Class(_readings, _result);

    [Benchmark]
    public void Struct() => CoreTemperatureClassVsStruct.Struct(_readingStructs, _result);

    [Benchmark]
    public void StructStackAlloc() => CoreTemperatureClassVsStruct.StructStackAlloc(_readingStructs, _result);
}

public static class CoreTemperatureClassVsStruct
{
    public static void Class(TemperatureReading[] readings, Dictionary<Phase, double[]> result)
    {
        var sums = new double[Cpu.PhaseCount * Cpu.CoreCount];

        foreach (var reading in readings)
        {
            sums[(int)reading.Phase * Cpu.CoreCount + reading.Core] += reading.Celsius;
        }

        CoreTemperatureResult.CopyFrom(sums, result);
    }

    public static void Struct(TemperatureReadingStruct[] readings, Dictionary<Phase, double[]> result)
    {
        var sums = new double[Cpu.PhaseCount * Cpu.CoreCount];

        foreach (var reading in readings)
        {
            sums[(int)reading.Phase * Cpu.CoreCount + reading.Core] += reading.Celsius;
        }

        CoreTemperatureResult.CopyFrom(sums, result);
    }

    public static void StructStackAlloc(TemperatureReadingStruct[] readings, Dictionary<Phase, double[]> result)
    {
        Span<double> sums = stackalloc double[Cpu.PhaseCount * Cpu.CoreCount];

        foreach (var reading in readings)
        {
            sums[(int)reading.Phase * Cpu.CoreCount + reading.Core] += reading.Celsius;
        }

        CoreTemperatureResult.CopyFrom(sums, result);
    }
}
