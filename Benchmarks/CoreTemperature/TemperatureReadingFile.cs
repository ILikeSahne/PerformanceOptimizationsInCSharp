using System.Globalization;

namespace Benchmarks.CoreTemperature;

public static class TemperatureReadingFile
{
    public static string[] ToLines(TemperatureReading[] readings)
    {
        return readings
            .Select(r => $"{r.Phase};{r.Celsius.ToString(CultureInfo.InvariantCulture)}")
            .ToArray();
    }

    public static List<TemperatureReading> LoadList(string[] lines)
    {
        var readings = new List<TemperatureReading>();

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            readings.Add(new TemperatureReading(
                Enum.Parse<Phase>(parts[0]),
                double.Parse(parts[1], CultureInfo.InvariantCulture)));
        }

        return readings;
    }

    public static TemperatureReading[] Load(string[] lines)
    {
        var readings = new List<TemperatureReading>();

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            readings.Add(new TemperatureReading(
                Enum.Parse<Phase>(parts[0]),
                double.Parse(parts[1], CultureInfo.InvariantCulture)));
        }

        return readings.ToArray();
    }

    public static TemperatureReadingStruct[] LoadStructs(string[] lines)
    {
        var readings = new List<TemperatureReadingStruct>();

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            readings.Add(new TemperatureReadingStruct(
                Enum.Parse<Phase>(parts[0]),
                double.Parse(parts[1], CultureInfo.InvariantCulture)));
        }

        return readings.ToArray();
    }
}
