using System.Runtime.InteropServices;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureDictionaryLookups
{
    public static Dictionary<Phase, int> NoLinq(List<TemperatureReading> readings)
    {
        var sums = new Dictionary<Phase, double>();
        var counts = new Dictionary<Phase, int>();

        foreach (var reading in readings)
        {
            sums[reading.Phase] = sums.GetValueOrDefault(reading.Phase) + reading.Celsius;
            counts[reading.Phase] = counts.GetValueOrDefault(reading.Phase) + 1;
        }

        var averages = sums.ToDictionary(s => s.Key, s => s.Value / counts[s.Key]);

        var spikes = new Dictionary<Phase, int>();

        foreach (var reading in readings)
        {
            if (reading.Celsius > averages[reading.Phase] + Spike.Threshold)
            {
                spikes[reading.Phase] = spikes.GetValueOrDefault(reading.Phase) + 1;
            }
        }

        return spikes;
    }

    public static Dictionary<Phase, int> FastLinq(List<TemperatureReading> readings)
    {
        var averages = readings
            .AggregateBy(
                r => r.Phase,
                (Sum: 0.0, Count: 0),
                (total, r) => (total.Sum + r.Celsius, total.Count + 1))
            .ToDictionary(kv => kv.Key, kv => kv.Value.Sum / kv.Value.Count);

        return readings
            .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
            .CountBy(r => r.Phase)
            .ToDictionary();
    }

    public static Dictionary<Phase, int> NoLinqValueRef(List<TemperatureReading> readings)
    {
        var sums = new Dictionary<Phase, double>();
        var counts = new Dictionary<Phase, int>();

        foreach (var reading in readings)
        {
            CollectionsMarshal.GetValueRefOrAddDefault(sums, reading.Phase, out _) += reading.Celsius;
            CollectionsMarshal.GetValueRefOrAddDefault(counts, reading.Phase, out _)++;
        }

        var averages = sums.ToDictionary(s => s.Key, s => s.Value / counts[s.Key]);

        var spikes = new Dictionary<Phase, int>();

        foreach (var reading in readings)
        {
            if (reading.Celsius > averages[reading.Phase] + Spike.Threshold)
            {
                CollectionsMarshal.GetValueRefOrAddDefault(spikes, reading.Phase, out _)++;
            }
        }

        return spikes;
    }
}
