using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Data;

namespace Benchmarks.CountPersonsOver50;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class CountPersonsOver50BenchmarkList
{
    private List<Person> _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        _persons = PersonFaker.Generate(10_000).ToList();
    }

    [Benchmark(Baseline = true)]
    public int WhereCount() => CountPersonsOver50DotNet10List.WhereCount(_persons);

    [Benchmark]
    public int Count() => CountPersonsOver50DotNet10List.Count(_persons);

    [Benchmark]
    public int ForEach() => CountPersonsOver50DotNet10List.ForEach(_persons);
}

public static class CountPersonsOver50DotNet10List
{
    public static int WhereCount(List<Person> persons)
    {
        return persons.Where(x => x.Age > 50).Count();
    }

    public static int Count(List<Person> persons)
    {
        return persons.Count(x => x.Age > 50);
    }

    public static int ForEach(List<Person> persons)
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
