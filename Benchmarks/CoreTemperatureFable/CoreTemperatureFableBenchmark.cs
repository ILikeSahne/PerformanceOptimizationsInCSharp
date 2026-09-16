using BenchmarkDotNet.Attributes;
using ClassVsStruct = Benchmarks.CoreTemperatureClassVsStruct.CoreTemperatureClassVsStruct;
using LinqVsArray = Benchmarks.CoreTemperatureLinqVsArray.CoreTemperatureLinqVsArray;
using Benchmarks.Data;

namespace Benchmarks.CoreTemperatureFable;

[MemoryDiagnoser]
public class CoreTemperatureFableBenchmark
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
    public void Linq() => LinqVsArray.Linq(_readings, _result);

    [Benchmark]
    public void Fable() => CoreTemperatureFable.Fable(_readings, _result);

    [Benchmark]
    public void FableStruct() => CoreTemperatureFable.FableStruct(_readingStructs, _result);

    [Benchmark]
    public void StructStackAlloc() => ClassVsStruct.StructStackAlloc(_readingStructs, _result);
}

public static class CoreTemperatureFable
{
    public static void Fable(TemperatureReading[] readings, Dictionary<Phase, double[]> result)
    {
        const int Slots = Cpu.PhaseCount * Cpu.CoreCount; // 96

        Span<double> sums = stackalloc double[Slots];
        Span<bool>   seen = stackalloc bool[Slots];

        foreach (var r in readings)
        {
            int idx = (int)r.Phase * Cpu.CoreCount + r.Core;
            sums[idx] += r.Celsius;
            seen[idx] = true;
        }

        for (int p = 0; p < Cpu.PhaseCount; p++)
        {
            double[]? target = null;
            int rowStart = p * Cpu.CoreCount;

            for (int c = 0; c < Cpu.CoreCount; c++)
            {
                int idx = rowStart + c;
                if (!seen[idx]) continue;

                target ??= result[(Phase)p];   // one dictionary lookup per phase, only if that phase occurred
                target[c] = sums[idx];
            }
        }
    }

    public static void FableStruct(TemperatureReadingStruct[] readings, Dictionary<Phase, double[]> result)
    {
        const int Slots = Cpu.PhaseCount * Cpu.CoreCount; // 96

        Span<double> sums = stackalloc double[Slots];
        Span<bool>   seen = stackalloc bool[Slots];

        foreach (ref readonly var r in readings.AsSpan())
        {
            int idx = (int)r.Phase * Cpu.CoreCount + r.Core;
            sums[idx] += r.Celsius;
            seen[idx] = true;
        }

        for (int p = 0; p < Cpu.PhaseCount; p++)
        {
            double[]? target = null;
            int rowStart = p * Cpu.CoreCount;

            for (int c = 0; c < Cpu.CoreCount; c++)
            {
                int idx = rowStart + c;
                if (!seen[idx]) continue;

                target ??= result[(Phase)p];
                target[c] = sums[idx];
            }
        }
    }
}
