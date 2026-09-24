using System.Runtime.InteropServices;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureArrayVsSpan
{
    public static Dictionary<Phase, int> NoLinqArray(List<TemperatureReading> readings)
    {
        var sums = new double[Phases.Count];
        var counts = new int[Phases.Count];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;
            sums[phase] += reading.Celsius;
            counts[phase]++;
        }

        var averages = sums.Select((sum, phase) => sum / counts[phase]).ToArray();

        var spikes = new int[Phases.Count];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;

            if (reading.Celsius > averages[phase] + Spike.Threshold)
            {
                spikes[phase]++;
            }
        }

        return PhaseDictionary.From(spikes);
    }

    public static Dictionary<Phase, int> NoLinqSpan(List<TemperatureReading> readings)
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
}
