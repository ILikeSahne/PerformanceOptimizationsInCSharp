using System.Globalization;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureLoading
{
    public static List<TemperatureReadingStruct> LoadStructs(string[] lines)
    {
        var readings = new List<TemperatureReadingStruct>();

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            readings.Add(new TemperatureReadingStruct(
                Enum.Parse<Phase>(parts[0]),
                double.Parse(parts[1], CultureInfo.InvariantCulture)));
        }

        return readings;
    }

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
}
