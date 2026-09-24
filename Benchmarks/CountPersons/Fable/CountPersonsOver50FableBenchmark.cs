using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.CountPersons;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class CountPersonsOver50FableBenchmark
{
    private IEnumerable<Person> _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        _persons = PersonFaker.Generate(10_000);
    }

    [Benchmark(Baseline = true)]
    public int WhereCount() => CountPersonsOver50Fable.WhereCount(_persons);

    [Benchmark]
    public int Count() => CountPersonsOver50Fable.Count(_persons);

    [Benchmark]
    public int ForEach() => CountPersonsOver50Fable.ForEach(_persons);

    [Benchmark]
    public int TypeSwitch() => CountPersonsOver50Fable.TypeSwitch(_persons);
}
