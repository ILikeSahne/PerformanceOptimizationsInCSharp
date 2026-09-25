namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureDictionaryVsArray
{
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

    public static Dictionary<Phase, int> NoLinqArray(List<TemperatureReading> readings)
    {
        var count = Enum.GetValues<Phase>().Length;

        var sums = new double[count];
        var counts = new int[count];

        foreach (var reading in readings)
        {
            var phase = (int)reading.Phase;
            sums[phase] += reading.Celsius;
            counts[phase]++;
        }

        var averages = sums.Select((sum, phase) => sum / counts[phase]).ToArray();

        var spikes = new int[count];

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
}
