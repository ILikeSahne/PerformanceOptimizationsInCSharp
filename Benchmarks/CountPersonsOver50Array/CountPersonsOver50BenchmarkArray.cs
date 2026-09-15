using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Data;

namespace Benchmarks.CountPersonsOver50;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class CountPersonsOver50BenchmarkArray
{
    private Person[] _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        _persons = PersonFaker.Generate(10_000).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int WhereCount() => CountPersonsOver50DotNet10Array.WhereCount(_persons);

    [Benchmark]
    public int Count() => CountPersonsOver50DotNet10Array.Count(_persons);

    [Benchmark]
    public int ForEach() => CountPersonsOver50DotNet10Array.ForEach(_persons);
}

public static class CountPersonsOver50DotNet10Array
{
    public static int WhereCount(Person[] persons)
    {
        return persons.Where(x => x.Age > 50).Count();
    }

    public static int Count(Person[] persons)
    {
        return persons.Count(x => x.Age > 50);
    }

    public static int ForEach(Person[] persons)
    {
        var count = 0;

        foreach (var person in persons)
        {
            if (person.Age > 50)
            {
                count++;
            }
        }

        return count;
    }
}
