using System.Globalization;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureLoadingAndSpikes
{
    public static List<TemperatureReadingStruct> LoadSpanSplit(string[] lines)
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

        return readings;
    }

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

    public static Dictionary<Phase, int> NoLinqArrayStaticCount(List<TemperatureReading> readings)
    {
        var sums = new double[Phases.Count];
        var counts = new int[Phases.Count];

        foreach (var reading in readings)
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

    public static Dictionary<Phase, int> NoLinqArrayStaticCount(List<TemperatureReadingStruct> readings)
    {
        var sums = new double[Phases.Count];
        var counts = new int[Phases.Count];

        foreach (var reading in readings)
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
}
