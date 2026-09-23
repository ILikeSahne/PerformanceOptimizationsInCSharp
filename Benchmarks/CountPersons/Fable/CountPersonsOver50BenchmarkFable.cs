using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.CountPersons;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class CountPersonsOver50BenchmarkFable
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
    public int TypeSwitch() => CountPersonsOver50Fable.TypeSwitch(_persons);
}

public static class CountPersonsOver50Fable
{
    public static int WhereCount(IEnumerable<Person> persons)
    {
        return persons.Where(x => x.Age > 50).Count();
    }

    public static int Count(IEnumerable<Person> persons)
    {
        return persons.Count(x => x.Age > 50);
    }

    public static int TypeSwitch(IEnumerable<Person> persons)
    {
        var count = 0;

        if (persons is Person[] array)
        {
            foreach (var person in array)
            {
                if (person.Age > 50)
                {
                    count++;
                }
            }
        }
        else if (persons is List<Person> list)
        {
            foreach (var person in list)
            {
                if (person.Age > 50)
                {
                    count++;
                }
            }
        }
        else
        {
            foreach (var person in persons)
            {
                if (person.Age > 50)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
