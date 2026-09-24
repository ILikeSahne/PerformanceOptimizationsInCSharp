using System.Runtime.InteropServices;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureFable
{
    public static Dictionary<Phase, int> Linq(List<TemperatureReading> readings)
    {
        var averages = readings
            .GroupBy(r => r.Phase)
            .ToDictionary(g => g.Key, g => g.Average(r => r.Celsius));

        return readings
            .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
            .GroupBy(r => r.Phase)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public static Dictionary<Phase, int> NoLinqSpan(List<TemperatureReadingStruct> readings)
    {
        var sums = new double[Phases.Count];
        var counts = new int[Phases.Count];

        foreach (var reading in CollectionsMarshal.AsSpan(readings))
        {
            var phase = (int)reading.Phase;
            sums[phase] += reading.Celsius;
            counts[phase]++;
        }

        var averages = new double[Phases.Count];

        for (var phase = 0; phase < Phases.Count; phase++)
        {
            averages[phase] = sums[phase] / counts[phase];
        }

        var spikes = new int[Phases.Count];

        foreach (var reading in CollectionsMarshal.AsSpan(readings))
        {
            var phase = (int)reading.Phase;

            if (reading.Celsius > averages[phase] + Spike.Threshold)
            {
                spikes[phase]++;
            }
        }

        return PhaseDictionary.From(spikes);
    }

    // TODO: add Fable's answers for the spikes version here
}
