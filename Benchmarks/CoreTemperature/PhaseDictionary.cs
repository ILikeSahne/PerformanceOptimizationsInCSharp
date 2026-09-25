namespace Benchmarks.CoreTemperature;

public static class PhaseDictionary
{
    public static Dictionary<Phase, int> From(ReadOnlySpan<int> values)
    {
        var result = new Dictionary<Phase, int>(values.Length);

        for (var phase = 0; phase < values.Length; phase++)
        {
            result[(Phase)phase] = values[phase];
        }

        return result;
    }
}
