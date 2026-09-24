using System.Runtime.InteropServices;

namespace Benchmarks.CountPersons;

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

    public static int ForEach(IEnumerable<Person> persons)
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

    public static int TypeSwitch(IEnumerable<Person> persons)
    {
        if (persons is List<Person> list)
            return CountOver50(CollectionsMarshal.AsSpan(list));

        if (persons is Person[] array)
            return CountOver50(array);

        return persons.Count(x => x.Age > 50);
    }

    private static int CountOver50(ReadOnlySpan<Person> span)
    {
        int count = 0;
        foreach (var p in span)
            if (p.Age > 50) count++;
        return count;
    }
}
