using System.Text;

namespace Benchmarks.CoreTemperature;

public static class CoreTemperatureSaving
{
    private static readonly int LineLength = "2;103.417205\n".Length;

    public static string Concat(List<TemperatureReadingStruct> spikes)
    {
        var result = "";

        foreach (var spike in spikes)
        {
            result += (int)spike.Phase + ";" + spike.Celsius + "\n";
        }

        return result;
    }

    public static string Builder(List<TemperatureReadingStruct> spikes)
    {
        var builder = new StringBuilder();

        foreach (var spike in spikes)
        {
            builder.Append((int)spike.Phase);
            builder.Append(';');
            builder.Append(spike.Celsius);
            builder.Append('\n');
        }

        return builder.ToString();
    }

    public static string BuilderExactSize(List<TemperatureReadingStruct> spikes)
    {
        var builder = new StringBuilder(spikes.Count * LineLength);

        foreach (var spike in spikes)
        {
            builder.Append((int)spike.Phase);
            builder.Append(';');
            builder.Append($"{spike.Celsius:000.000000}");
            builder.Append('\n');
        }

        return builder.ToString();
    }

    public static string StringCreate(List<TemperatureReadingStruct> spikes)
    {
        return string.Create(spikes.Count * LineLength, spikes, (span, spikes) =>
        {
            foreach (var spike in spikes)
            {
                ((int)spike.Phase).TryFormat(span[0..1], out _);
                span[1] = ';';
                spike.Celsius.TryFormat(span[2..12], out _, "000.000000");
                span[12] = '\n';
                span = span[LineLength..];
            }
        });
    }
}
