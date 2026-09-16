namespace Benchmarks.Data;

public static class CoreTemperatureResult
{
    public static Dictionary<Phase, double[]> Create()
    {
        var result = new Dictionary<Phase, double[]>();

        foreach (var phase in Enum.GetValues<Phase>())
        {
            result[phase] = new double[Cpu.CoreCount];
        }

        return result;
    }

    public static void CopyFrom(ReadOnlySpan<double> sums, Dictionary<Phase, double[]> result)
    {
        foreach (var (phase, coreSums) in result)
        {
            sums.Slice((int)phase * Cpu.CoreCount, Cpu.CoreCount).CopyTo(coreSums);
        }
    }
}
