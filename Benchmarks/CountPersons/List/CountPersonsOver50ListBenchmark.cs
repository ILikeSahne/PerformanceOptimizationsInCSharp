using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.CountPersons;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class CountPersonsOver50ListBenchmark
{
    private List<Person> _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        _persons = PersonFaker.Generate(10_000);
    }

    [Benchmark(Baseline = true)]
    public int WhereCount() => CountPersonsOver50List.WhereCount(_persons);

    [Benchmark]
    public int Count() => CountPersonsOver50List.Count(_persons);

    [Benchmark]
    public int ForEach() => CountPersonsOver50List.ForEach(_persons);
}
