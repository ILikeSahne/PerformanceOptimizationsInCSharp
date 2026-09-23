namespace Benchmarks.CoreTemperature;

public static class PhaseDictionary
{
    public static Dictionary<Phase, int> From(ReadOnlySpan<int> values)
    {
        var result = new Dictionary<Phase, int>();

        foreach (var phase in Enum.GetValues<Phase>())
        {
            result[phase] = values[(int)phase];
        }

        return result;
    }
}
